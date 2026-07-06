using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VictronBridge.Configuration.Sources;

namespace VictronBridge.Configuration;

public static class ConfigurationServiceCollectionExtensions
{
    public static IServiceCollection AddBridgeConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<BridgeOptions>()
            .Bind(configuration.GetSection(BridgeOptions.SectionName))
            .ValidateOnStart();

        services
            .AddOptions<MqttSourceOptions>()
            .Bind(configuration.GetSection(MqttSourceOptions.SectionName));

        services
            .AddOptions<ModbusTcpSourceOptions>()
            .Bind(configuration.GetSection(ModbusTcpSourceOptions.SectionName));

        services
            .AddOptions<ModbusRtuSourceOptions>()
            .Bind(configuration.GetSection(ModbusRtuSourceOptions.SectionName));

        services
            .AddOptions<HttpSourceOptions>()
            .Bind(configuration.GetSection(HttpSourceOptions.SectionName));

        services.AddSingleton<IValidateOptions<BridgeOptions>, BridgeOptionsValidator>();

        return services;
    }
}
