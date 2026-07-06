namespace VictronBridge.Configuration;

public sealed class BridgeOptions
{
    public const string SectionName = "Bridge";

    public SourceOptions Source { get; set; } = new();
    public DeviceOptions Device { get; set; } = new();
    public Dictionary<string, string> Mappings { get; set; } = new();
}
