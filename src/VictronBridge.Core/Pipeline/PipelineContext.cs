namespace VictronBridge.Core.Pipeline;

public sealed class PipelineContext
{
    public string SourceName { get; init; } = string.Empty;
    public IReadOnlyDictionary<string, object?> SourceValues { get; set; } = new Dictionary<string, object?>();
    public IReadOnlyDictionary<string, object?> MappedValues { get; set; } = new Dictionary<string, object?>();
}
