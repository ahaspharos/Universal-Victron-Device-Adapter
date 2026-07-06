namespace VictronBridge.Configuration.Sources;

public sealed class HttpSourceOptions
{
    public const string SectionName = "Bridge:Source:Http";

    public string BaseUrl { get; set; } = string.Empty;
    public int PollingIntervalSeconds { get; set; } = 30;
    public Dictionary<string, string> Headers { get; set; } = new();
}
