using Microsoft.Extensions.DependencyInjection;
using SecureP.Service.Abstraction;

namespace SecureP.Service.TokenService.Extensions;

public static class ParkingLocationServiceExtensions
{
    public static IServiceCollection AddParkingLocationService<TKey>(this IServiceCollection services) where TKey : IEquatable<TKey>
    {
        /* services.AddScoped<IParkingLocationService, ParkingLocationService.ParkingLocationService<TKey>>(); */

        return services;
    }
}