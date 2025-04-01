using Microsoft.Extensions.DependencyInjection;
using SecureP.Repository.Abstraction;

namespace SecureP.Repository.ParkingLocations.Extensions;

public static class ParkingLocationRepositoryExtensions
{
    public static IServiceCollection AddParkingLocationRepository<TKey>(this IServiceCollection services) where TKey : IEquatable<TKey>
    {
        services.AddScoped<IParkingLocationRepository<TKey>, ParkingLocationRepository<TKey>>();

        return services;
    }
}