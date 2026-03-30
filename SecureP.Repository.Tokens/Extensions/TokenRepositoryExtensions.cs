using Microsoft.Extensions.DependencyInjection;
using SecureP.Repository.Abstraction;

namespace SecureP.Repository.Tokens.Extensions;

public static class TokenRepositoryExtensions
{
    public static IServiceCollection AddTokenRepository<TKey>(this IServiceCollection services) where TKey : IEquatable<TKey>
    {
        return services.AddScoped<ITokenRepository<TKey>, TokenRepository<TKey>>();
    }
}