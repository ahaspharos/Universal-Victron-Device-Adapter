using Microsoft.Extensions.DependencyInjection;
using VictronBridge.Core.Abstractions;

namespace VictronBridge.Devices.PvInverter;

public static class PvInverterServiceCollectionExtensions
{
    public static IServiceCollection AddPvInverterDevice(this IServiceCollection services)
    {
        services.AddSingleton<IDeviceModel, PvInverterDeviceModel>();
        return services;
    }
}
