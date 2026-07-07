using Microsoft.Extensions.Options;
using VictronBridge.Configuration;
using VictronBridge.Core.Abstractions;
using VictronBridge.Core.Events;
using VictronBridge.DBus;

namespace VictronBridge.Host;

public sealed class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IDataSource _source;
    private readonly IMappingEngine _mappingEngine;
    private readonly IDeviceModel _deviceModel;
    private readonly IDbusPublisher _dbusPublisher;
    private readonly BridgeOptions _options;

    // Accumulates raw topic→value pairs from the source adapter
    private readonly Dictionary<string, object?> _sourceValues = new(StringComparer.OrdinalIgnoreCase);
    private readonly SemaphoreSlim _gate = new(1, 1);

    public Worker(
        ILogger<Worker> logger,
        IDataSource source,
        IMappingEngine mappingEngine,
        IDeviceModel deviceModel,
        IDbusPublisher dbusPublisher,
        IOptions<BridgeOptions> options)
    {
        _logger = logger;
        _source = source;
        _mappingEngine = mappingEngine;
        _deviceModel = deviceModel;
        _dbusPublisher = dbusPublisher;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "VictronBridge started. Source={Source} Device={Device}",
            _options.Source.Type,
            _options.Device.ServiceName);

        // Initialise D-Bus once at startup with the seeded device model values
        if (_dbusPublisher is VenusDbusPublisher venusPublisher)
        {
            await venusPublisher.InitialiseAsync(
                _deviceModel.ServiceName,
                _deviceModel.GetDbusValues(),
                stoppingToken);
        }

        _source.ValueReceived += OnValueReceived;

        try
        {
            await _source.StartAsync(stoppingToken);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // Normal shutdown
        }
        finally
        {
            _source.ValueReceived -= OnValueReceived;
            await _source.StopAsync(CancellationToken.None);

            if (_dbusPublisher is IAsyncDisposable disposable)
                await disposable.DisposeAsync();
        }
    }

    private void OnValueReceived(object? sender, SourceValueReceivedEventArgs e)
    {
        _gate.Wait();
        try
        {
            _sourceValues[e.Key] = e.Value;
        }
        finally
        {
            _gate.Release();
        }

        _ = PushToPipelineAsync();
    }

    private async Task PushToPipelineAsync()
    {
        await _gate.WaitAsync();
        IReadOnlyDictionary<string, object?> snapshot;
        try
        {
            snapshot = new Dictionary<string, object?>(_sourceValues, StringComparer.OrdinalIgnoreCase);
        }
        finally
        {
            _gate.Release();
        }

        try
        {
            var mapped = _mappingEngine.Map(snapshot, _options.Mappings);
            _deviceModel.ApplyValues(mapped);

            var dbusValues = _deviceModel.GetDbusValues();            
            await _dbusPublisher.PublishAsync(_deviceModel.ServiceName, dbusValues);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Pipeline error.");
        }
    }
}
