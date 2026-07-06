namespace VictronBridge.Configuration.Sources;

public sealed class ModbusTcpSourceOptions
{
    public const string SectionName = "Bridge:Source:ModbusTcp";

    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 502;
    public byte UnitId { get; set; } = 1;
    public int PollingIntervalSeconds { get; set; } = 5;
}
