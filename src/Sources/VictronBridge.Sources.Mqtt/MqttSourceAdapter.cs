using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using VictronBridge.Configuration.Sources;
using VictronBridge.Core.Abstractions;
using VictronBridge.Core.Events;

namespace VictronBridge.Sources.Mqtt;

public sealed class MqttSourceAdapter : IDataSource, IAsyncDisposable
{
    private readonly MqttSourceOptions _options;
    private readonly ILogger<MqttSourceAdapter> _logger;
    private readonly MqttClientFactory _factory;
    private IMqttClient? _client;

    public string Name => "MQTT";

    public event EventHandler<SourceValueReceivedEventArgs>? ValueReceived;

    public MqttSourceAdapter(
        IOptions<MqttSourceOptions> options,
        ILogger<MqttSourceAdapter> logger)
    {
        _options = options.Value;
        _logger = logger;
        _factory = new MqttClientFactory();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _client = _factory.CreateMqttClient();

        _client.ConnectedAsync += OnConnectedAsync;
        _client.DisconnectedAsync += OnDisconnectedAsync;
        _client.ApplicationMessageReceivedAsync += OnMessageReceivedAsync;

        await ConnectAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_client is null || !_client.IsConnected)
            return;

        await _client.DisconnectAsync(
            new MqttClientDisconnectOptions
            {
                Reason = MqttClientDisconnectOptionsReason.NormalDisconnection
            },
            cancellationToken);
    }

    private async Task ConnectAsync(CancellationToken cancellationToken)
    {
        var optionsBuilder = new MqttClientOptionsBuilder()
            .WithTcpServer(_options.Host, _options.Port > 0 ? _options.Port : 1883)
            .WithKeepAlivePeriod(TimeSpan.FromSeconds(
                _options.KeepAliveSeconds > 0 ? _options.KeepAliveSeconds : 60));

        if (!string.IsNullOrWhiteSpace(_options.ClientId))
            optionsBuilder = optionsBuilder.WithClientId(_options.ClientId);

        if (!string.IsNullOrWhiteSpace(_options.Username))
            optionsBuilder = optionsBuilder.WithCredentials(_options.Username, _options.Password);

        if (_options.UseTls)
            optionsBuilder = optionsBuilder.WithTlsOptions(o => o.UseTls());

        var mqttOptions = optionsBuilder.Build();

        _logger.LogInformation("Connecting to MQTT broker {Host}:{Port}...",
            _options.Host, _options.Port);

        await _client!.ConnectAsync(mqttOptions, cancellationToken);
    }

    private async Task OnConnectedAsync(MqttClientConnectedEventArgs args)
    {
        _logger.LogInformation("Connected to MQTT broker.");
        await SubscribeAsync(CancellationToken.None);
    }

    private async Task OnDisconnectedAsync(MqttClientDisconnectedEventArgs args)
    {
        if (args.ClientWasConnected)
            _logger.LogWarning("Disconnected from MQTT broker. Reason: {Reason}", args.Reason);

        if (args.Exception is not null)
            _logger.LogError(args.Exception, "MQTT disconnect caused by exception.");

        await Task.Delay(TimeSpan.FromSeconds(5));

        try
        {
            await ConnectAsync(CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Reconnect attempt failed.");
        }
    }

    private Task OnMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs args)
    {
        var topic = args.ApplicationMessage.Topic;
        var payload = args.ApplicationMessage.ConvertPayloadToString() ?? string.Empty;

        var value = MqttTopicValueParser.Parse(payload);

        _logger.LogDebug("MQTT message received: {Topic} = {Value}", topic, value);

        ValueReceived?.Invoke(this, new SourceValueReceivedEventArgs(
            topic, value, DateTimeOffset.UtcNow));

        return Task.CompletedTask;
    }

    private async Task SubscribeAsync(CancellationToken cancellationToken)
    {
        if (_options.Topics.Count == 0)
        {
            _logger.LogWarning("No MQTT topics configured. Nothing to subscribe to.");
            return;
        }

        var subscribeOptionsBuilder = new MqttClientSubscribeOptionsBuilder();
        foreach (var topic in _options.Topics)
            subscribeOptionsBuilder.WithTopicFilter(topic);

        await _client!.SubscribeAsync(subscribeOptionsBuilder.Build(), cancellationToken);

        _logger.LogInformation("Subscribed to {Count} topic(s): {Topics}",
            _options.Topics.Count, string.Join(", ", _options.Topics));
    }

    public async ValueTask DisposeAsync()
    {
        if (_client is not null)
        {
            _client.ConnectedAsync -= OnConnectedAsync;
            _client.DisconnectedAsync -= OnDisconnectedAsync;
            _client.ApplicationMessageReceivedAsync -= OnMessageReceivedAsync;
            _client.Dispose();
            _client = null;
        }

        await ValueTask.CompletedTask;
    }
}
