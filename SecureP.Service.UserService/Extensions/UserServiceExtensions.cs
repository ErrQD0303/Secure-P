using Microsoft.Extensions.DependencyInjection;
using SecureP.Repository.UserRepository.Extensions;
using SecureP.Service.Abstraction;

namespace SecureP.Service.UserService.Extensions;

public static class UserServiceExtensions
{
    public static IServiceCollection AddUserService<TKey>(this IServiceCollection services) where TKey : IEquatable<TKey>
    {
        services.AddUserRepository<TKey>();
        services.AddScoped<IUserService<TKey>, UserService<TKey>>();

        return services;
    }
}