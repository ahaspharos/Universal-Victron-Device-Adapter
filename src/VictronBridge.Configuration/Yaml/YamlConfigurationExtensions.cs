using Microsoft.Extensions.Configuration;

namespace VictronBridge.Configuration.Yaml;

public static class YamlConfigurationExtensions
{
    public static IConfigurationBuilder AddYamlFile(
        this IConfigurationBuilder builder,
        string path,
        bool optional = false,
        bool reloadOnChange = false)
    {
        return builder.Add(new YamlConfigurationSource
        {
            Path = path,
            Optional = optional,
            ReloadOnChange = reloadOnChange
        });
    }

    public static IConfigurationBuilder AddYamlStream(
        this IConfigurationBuilder builder,
        Stream stream)
    {
        return builder.Add(new StreamYamlConfigurationSource(stream));
    }
}
