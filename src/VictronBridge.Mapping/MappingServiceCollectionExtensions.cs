using Microsoft.Extensions.DependencyInjection;
using VictronBridge.Core.Abstractions;

namespace VictronBridge.Mapping;

public static class MappingServiceCollectionExtensions
{
    public static IServiceCollection AddMappingEngine(this IServiceCollection services)
    {
        services.AddSingleton<IMappingEngine, MappingEngine>();
        return services;
    }
}
