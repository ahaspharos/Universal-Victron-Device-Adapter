namespace VictronBridge.Configuration.Sources;

public sealed class ModbusRtuSourceOptions
{
    public const string SectionName = "Source:ModbusRtu";

    public string SerialPort { get; set; } = string.Empty;
    public int BaudRate { get; set; } = 9600;
    public byte UnitId { get; set; } = 1;
    public int PollingIntervalSeconds { get; set; } = 5;
}
