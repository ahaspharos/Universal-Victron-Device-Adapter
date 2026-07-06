using VictronBridge.Core.Abstractions;

namespace VictronBridge.Mapping;

public sealed class MappingEngine : IMappingEngine
{
    public IReadOnlyDictionary<string, object?> Map(
        IReadOnlyDictionary<string, object?> sourceValues,
        IEnumerable<KeyValuePair<string, string>> mappings)
    {
        var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        foreach (var (deviceKey, sourceTopic) in mappings)
        {
            if (sourceValues.TryGetValue(sourceTopic, out var value))
                result[deviceKey] = value;
        }

        return result;
    }
}
