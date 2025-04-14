using Microsoft.Extensions.DependencyInjection;
using SecureP.Repository.ParkingLocations.Extensions;
using SecureP.Service.Abstraction;

namespace SecureP.Service.TokenService.Extensions;

public static class ParkingLocationServiceExtensions
{
    public static IServiceCollection AddParkingLocationService<TKey>(this IServiceCollection services) where TKey : class, IEquatable<TKey>
    {
        services.AddParkingLocationRepository<TKey>();
        services.AddScoped<IParkingLocationService<TKey>, ParkingLocationService.ParkingLocationService<TKey>>();

        return services;
    }
}