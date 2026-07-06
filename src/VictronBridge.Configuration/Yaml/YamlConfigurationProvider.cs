using Microsoft.Extensions.Configuration;

namespace VictronBridge.Configuration.Yaml;

public sealed class YamlConfigurationProvider : FileConfigurationProvider
{
    public YamlConfigurationProvider(YamlConfigurationSource source)
        : base(source) { }

    public override void Load(Stream stream)
    {
        Data = YamlParser.Flatten(stream);
    }
}
