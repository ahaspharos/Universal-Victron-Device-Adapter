namespace VictronBridge.Configuration.Sources;

public sealed class MqttSourceOptions
{
    public const string SectionName = "Bridge:Source:Mqtt";

    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 1883;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public bool UseTls { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public int KeepAliveSeconds { get; set; } = 60;
    public List<string> Topics { get; set; } = new();
}
