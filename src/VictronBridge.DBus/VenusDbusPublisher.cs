using Microsoft.Extensions.Logging;
using Tmds.DBus.Protocol;
using VictronBridge.Core.Abstractions;

namespace VictronBridge.DBus;

public sealed class VenusDbusPublisher : IDbusPublisher, IAsyncDisposable
{
    private readonly ILogger<VenusDbusPublisher> _logger;
    private DBusConnection? _connection;
    private string _serviceName = string.Empty;

    // path → handler; populated on first Publish
    private readonly Dictionary<string, VictronBusItemHandler> _handlers = new(StringComparer.OrdinalIgnoreCase);
    private VictronRootHandler? _rootHandler;
    private bool _registered;

    public VenusDbusPublisher(ILogger<VenusDbusPublisher> logger)
    {
        _logger = logger;
    }

    public async Task PublishAsync(
        string serviceName,
        IReadOnlyDictionary<string, object?> values,
        CancellationToken cancellationToken = default)
    {
        if (!_registered)
            await RegisterAsync(serviceName, values, cancellationToken);
        else
            await UpdateValuesAsync(values, cancellationToken);
    }

    private async Task RegisterAsync(
        string serviceName,
        IReadOnlyDictionary<string, object?> values,
        CancellationToken cancellationToken)
    {
        _serviceName = serviceName;

        _logger.LogInformation("Connecting to D-Bus system bus...");

        var address = DBusAddress.System ?? "unix:path=/run/dbus/system_bus_socket";
        _connection = new DBusConnection(address);
        await _connection.ConnectAsync();

        _logger.LogInformation("Connected to D-Bus. Requesting service name '{ServiceName}'...", serviceName);

        await _connection.RequestNameAsync(serviceName, RequestNameOptions.None);

        _logger.LogInformation("Service name '{ServiceName}' registered.", serviceName);

        // Build handlers for all current paths
        foreach (var (path, value) in values)
        {
            var handler = new VictronBusItemHandler(path, serviceName, value);
            _handlers[path] = handler;
        }

        // Register root handler
        _rootHandler = new VictronRootHandler(serviceName, () =>
            _handlers.ToDictionary(h => h.Key, h => h.Value.CurrentValue));

        // Register all handlers at once
        var allHandlers = new List<IPathMethodHandler> { _rootHandler };
        allHandlers.AddRange(_handlers.Values);
        _connection.AddMethodHandlers(allHandlers);

        _registered = true;
        _logger.LogInformation("Published {Count} D-Bus paths for '{ServiceName}'.", values.Count, serviceName);
    }

    private Task UpdateValuesAsync(
        IReadOnlyDictionary<string, object?> values,
        CancellationToken cancellationToken)
    {
        if (_connection is null)
            return Task.CompletedTask;

        foreach (var (path, value) in values)
        {
            if (_handlers.TryGetValue(path, out var handler))
            {
                handler.UpdateValue(value);
            }
            else
            {
                // New path added after initial registration
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
            if (_registered && !string.IsNullOrEmpty(_serviceName))
            {
                try { await _connection.ReleaseNameAsync(_serviceName); }
                catch { /* best-effort */ }
            }

            _connection.Dispose();
            _connection = null;
        }
    }
}
