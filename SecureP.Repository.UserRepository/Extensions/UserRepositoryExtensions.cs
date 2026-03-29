using Microsoft.Extensions.DependencyInjection;
using SecureP.Repository.Abstraction;

namespace SecureP.Repository.UserRepository.Extensions;

public static class UserRepositoryExtensions
{
    public static IServiceCollection AddUserRepository<TKey>(this IServiceCollection services) where TKey : IEquatable<TKey>
    {
        return services.AddScoped<IUserRepository<TKey>, UserRepository<TKey>>();
    }
}