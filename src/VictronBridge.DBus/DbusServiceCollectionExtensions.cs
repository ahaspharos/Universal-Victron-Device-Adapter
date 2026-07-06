using Microsoft.Extensions.DependencyInjection;
using VictronBridge.Core.Abstractions;

namespace VictronBridge.DBus;

public static class DbusServiceCollectionExtensions
{
    public static IServiceCollection AddVenusDbusPublisher(this IServiceCollection services)
    {
        services.AddSingleton<VenusDbusPublisher>();
        services.AddSingleton<IDbusPublisher>(sp => sp.GetRequiredService<VenusDbusPublisher>());
        return services;
    }
}
