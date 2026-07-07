using Microsoft.Extensions.Logging;
using Tmds.DBus.Protocol;
using VictronBridge.Core.Abstractions;

namespace VictronBridge.DBus;

public sealed class VenusDbusPublisher : IDbusPublisher, IAsyncDisposable
{
    private readonly ILogger<VenusDbusPublisher> _logger;
    private DBusConnection? _connection;
    private string _serviceName = string.Empty;

    private readonly Dictionary<string, VictronBusItemHandler> _handlers = new(StringComparer.OrdinalIgnoreCase);
    private VictronRootHandler? _rootHandler;
    private bool _initialised;

    public VenusDbusPublisher(ILogger<VenusDbusPublisher> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Connects to the D-Bus system bus and registers the service name.
    /// Must be called once at startup before any calls to PublishAsync.
    /// </summary>
    public async Task InitialiseAsync(
        string serviceName,
        IReadOnlyDictionary<string, object?> initialValues,
        CancellationToken cancellationToken = default)
    {
        if (_initialised)
            return;

        if (string.IsNullOrWhiteSpace(serviceName))
            throw new InvalidOperationException(
                "D-Bus service name is empty. Ensure Bridge:Device:ServiceName is set in configuration.");

        _serviceName = serviceName;

        _logger.LogInformation("Connecting to D-Bus system bus...");

        var address = DBusAddress.System ?? "unix:path=/run/dbus/system_bus_socket";
        _connection = new DBusConnection(address);

        try
        {
            await _connection.ConnectAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to D-Bus system bus.");
            _connection.Dispose();
            _connection = null;
            throw;
        }

        _logger.LogInformation("Connected to D-Bus. Requesting service name '{ServiceName}'...", serviceName);

        try
        {
            await _connection.RequestNameAsync(serviceName, RequestNameOptions.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register D-Bus service name '{ServiceName}'.", serviceName);
            _connection.Dispose();
            _connection = null;
            throw;
        }

        _logger.LogInformation("Service name '{ServiceName}' registered.", serviceName);

        // Build a handler per D-Bus path from the initial device model snapshot
        foreach (var (path, value) in initialValues)
        {
            var handler = new VictronBusItemHandler(path, serviceName, value);
            _handlers[path] = handler;
        }

        _rootHandler = new VictronRootHandler(serviceName, () =>
            _handlers.ToDictionary(h => h.Key, h => h.Value.CurrentValue));

        var allHandlers = new List<IPathMethodHandler> { _rootHandler };
        allHandlers.AddRange(_handlers.Values);
        _connection.AddMethodHandlers(allHandlers);

        _initialised = true;
        _logger.LogInformation("Registered {Count} D-Bus path(s) for '{ServiceName}'.", initialValues.Count, serviceName);
    }

    public Task PublishAsync(
        string serviceName,
        IReadOnlyDictionary<string, object?> values,
        CancellationToken cancellationToken = default)
    {
        if (!_initialised || _connection is null)
        {
            _logger.LogWarning("PublishAsync called before InitialiseAsync — values dropped.");
            return Task.CompletedTask;
        }

        foreach (var (path, value) in values)
        {
            if (_handlers.TryGetValue(path, out var handler))
            {
                handler.UpdateValue(value);
            }
            else
            {
                var newHandler = new VictronBusItemHandler(path, _serviceName, value);
                _handlers[path] = newHandler;
                _connection.AddMethodHandler(newHandler);
                _logger.LogDebug("Registered new D-Bus path '{Path}'.", path);
            }

            EmitPropertiesChanged(path);
        }

        return Task.CompletedTask;
    }

    private void EmitPropertiesChanged(string path)
    {
        if (_connection is null || !_handlers.TryGetValue(path, out var handler))
            return;

        try
        {
            var signal = handler.CreatePropertiesChangedSignal(_connection);
            _connection.TrySendMessage(signal);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to emit PropertiesChanged for path '{Path}'.", path);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            if (_initialised && !string.IsNullOrEmpty(_serviceName))
            {
                try { await _connection.ReleaseNameAsync(_serviceName); }
                catch { /* best-effort */ }
            }

            _connection.Dispose();
            _connection = null;
        }
    }
}
