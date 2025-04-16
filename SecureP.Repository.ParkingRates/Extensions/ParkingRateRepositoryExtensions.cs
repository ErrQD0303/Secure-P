using Microsoft.Extensions.DependencyInjection;
using SecureP.Repository.Abstraction;

namespace SecureP.Repository.ParkingRates.Extensions;

public static class ParkingRateRepositoryExtensions
{
    public static IServiceCollection AddParkingRateRepository<TKey>(this IServiceCollection services) where TKey : IEquatable<TKey>
    {
        services.AddScoped<IParkingRateRepository<TKey>, ParkingRateRepository<TKey>>();

        return services;
    }
}