using Microsoft.Extensions.DependencyInjection;
using SecureP.Repository.ParkingRates.Extensions;
using SecureP.Service.Abstraction;

namespace SecureP.Service.ParkingRateService.Extensions;

public static class ParkingRateServiceExtensions
{
    public static IServiceCollection AddParkingRateService<TKey>(this IServiceCollection services) where TKey : IEquatable<TKey>
    {
        services.AddParkingRateRepository<TKey>();
        services.AddTransient<IParkingRateService<TKey>, ParkingRateService<TKey>>();

        return services;
    }
}