namespace VictronBridge.Configuration;

public sealed class SourceOptions
{
    public string Type { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public bool UseTls { get; set; }
    public Dictionary<string, string> AdditionalProperties { get; set; } = new();
}
