using Microsoft.Extensions.DependencyInjection;
using VictronBridge.Core.Abstractions;

namespace VictronBridge.Sources.Mqtt;

public static class MqttServiceCollectionExtensions
{
    public static IServiceCollection AddMqttSource(this IServiceCollection services)
    {
        services.AddSingleton<MqttSourceAdapter>();
        services.AddSingleton<IDataSource>(sp => sp.GetRequiredService<MqttSourceAdapter>());
        return services;
    }
}
