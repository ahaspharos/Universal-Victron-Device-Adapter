namespace VictronBridge.Core.Abstractions;

public interface IMappingEngine
{
    IReadOnlyDictionary<string, object?> Map(
        IReadOnlyDictionary<string, object?> sourceValues,
        IEnumerable<KeyValuePair<string, string>> mappings);
}
