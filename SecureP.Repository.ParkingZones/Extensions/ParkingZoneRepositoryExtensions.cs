using Microsoft.Extensions.DependencyInjection;
using SecureP.Repository.Abstraction;
using SecureP.Repository.ParkingZones;

namespace SecureP.Repository.ParkingRates.Extensions;

public static class ParkingZoneRepositoryExtensions
{
    public static IServiceCollection AddParkingZoneRepository<TKey>(this IServiceCollection services) where TKey : IEquatable<TKey>
    {
        services.AddScoped<IParkingZoneRepository<TKey>, ParkingZoneRepository<TKey>>();

        return services;
    }
}