using YamlDotNet.RepresentationModel;

namespace VictronBridge.Configuration.Yaml;

internal static class YamlParser
{
    internal static Dictionary<string, string?> Flatten(string yaml)
    {
        var stream = new YamlStream();
        using var reader = new StringReader(yaml);
        stream.Load(reader);
        return Flatten(stream);
    }

    internal static Dictionary<string, string?> Flatten(Stream stream)
    {
        var yamlStream = new YamlStream();
        using var reader = new StreamReader(stream, leaveOpen: true);
        yamlStream.Load(reader);
        return Flatten(yamlStream);
    }

    private static Dictionary<string, string?> Flatten(YamlStream yamlStream)
    {
        var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        if (yamlStream.Documents.Count > 0 && yamlStream.Documents[0].RootNode is YamlMappingNode root)
            FlattenNode(root, string.Empty, data);

        return data;
    }

    private static void FlattenNode(
        YamlNode node,
        string prefix,
        Dictionary<string, string?> data)
    {
        switch (node)
        {
            case YamlMappingNode mapping:
                foreach (var entry in mapping.Children)
                {
                    var key = entry.Key is YamlScalarNode scalar ? scalar.Value ?? string.Empty : string.Empty;
                    var childKey = string.IsNullOrEmpty(prefix) ? key : $"{prefix}:{key}";
                    FlattenNode(entry.Value, childKey, data);
                }
                break;

            case YamlSequenceNode sequence:
                for (var i = 0; i < sequence.Children.Count; i++)
                    FlattenNode(sequence.Children[i], $"{prefix}:{i}", data);
                break;

            case YamlScalarNode scalar:
                data[prefix] = scalar.Value;
                break;
        }
    }
}
