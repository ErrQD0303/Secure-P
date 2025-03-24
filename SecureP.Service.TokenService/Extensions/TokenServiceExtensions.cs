using Microsoft.Extensions.DependencyInjection;
using SecureP.Repository.Tokens.Extensions;
using SecureP.Service.Abstraction;

namespace SecureP.Service.TokenService.Extensions;

public static class TokenServiceExtensions
{
    public static IServiceCollection AddTokenService<TKey>(this IServiceCollection services) where TKey : IEquatable<TKey>
    {
        services.AddTokenRepository<TKey>();
        services.AddScoped<ITokenService<TKey>, TokenService<TKey>>();
        services.AddSingleton<JwtGenerator>();

        return services;
    }
}