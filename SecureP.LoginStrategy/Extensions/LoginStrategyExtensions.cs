using Microsoft.Extensions.DependencyInjection;
using SecureP.LoginStrategy.Abstraction;

namespace SecureP.LoginStrategy.Extensions;

public static class LoginStrategyExtensions
{
    public static void AddLoginStrategies<TKey>(this IServiceCollection services)
        where TKey : IEquatable<TKey>
    {
        services.AddScoped<ILoginStrategy<TKey>, EmailLoginStrategy<TKey>>();
        services.AddScoped<ILoginStrategy<TKey>, UsernameLoginStrategy<TKey>>();
        services.AddScoped<ILoginStrategyFactory<TKey>, LoginStrategyFactory<TKey>>();
    }
}