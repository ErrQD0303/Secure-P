using Microsoft.Extensions.DependencyInjection;
using SecureP.Repository.ParkingRates.Extensions;
using SecureP.Service.Abstraction;

namespace SecureP.Service.ParkingZoneService.Extensions;

public static class ParkingZoneServiceExtensions
{
    public static IServiceCollection AddParkingZoneService<TKey>(this IServiceCollection services) where TKey : IEquatable<TKey>
    {
        services.AddParkingZoneRepository<TKey>();
        services.AddTransient<IParkingZoneService<TKey>, ParkingZoneService<TKey>>();

        return services;
    }
}