using Microsoft.Extensions.DependencyInjection;
using VictronBridge.Core.Abstractions;

namespace VictronBridge.Devices.Battery;

public static class BatteryServiceCollectionExtensions
{
    public static IServiceCollection AddBatteryDevice(this IServiceCollection services)
    {
        services.AddSingleton<IDeviceModel, BatteryDeviceModel>();
        return services;
    }
}
