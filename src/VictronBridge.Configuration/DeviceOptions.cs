namespace VictronBridge.Configuration;

public sealed class DeviceOptions
{
    public string Type { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string ProductName { get; set; } = "VictronBridge Device";
    public int DeviceInstance { get; set; }
    public Dictionary<string, string> AdditionalProperties { get; set; } = new();
}
