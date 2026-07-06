using Microsoft.Extensions.Configuration;

namespace VictronBridge.Configuration.Yaml;

internal sealed class StreamYamlConfigurationSource : IConfigurationSource
{
    private readonly Stream _stream;

    public StreamYamlConfigurationSource(Stream stream)
    {
        _stream = stream;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
        => new StreamYamlConfigurationProvider(_stream);
}

internal sealed class StreamYamlConfigurationProvider : ConfigurationProvider
{
    private readonly Stream _stream;

    public StreamYamlConfigurationProvider(Stream stream)
    {
        _stream = stream;
    }

    public override void Load()
    {
        _stream.Position = 0;
        Data = YamlParser.Flatten(_stream);
    }
}
