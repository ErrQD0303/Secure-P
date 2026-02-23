
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using SecureP.Identity.Models.Enum;

namespace SecureP.Authorization;

/// <summary>
/// Extension methods for configuring authorization services in the application
/// </summary>
public static class AuthorizationExtensions
{
    /// <summary>
    /// Adds permission-based authorization services to the application's dependency injection container. This method registers the necessary authorization handlers and policies based on the defined RoleClaimType enumeration. Each role claim type is added as a policy, allowing for fine-grained control over access to resources based on user roles and permissions. By calling this extension method during service configuration, you can easily set up a robust authorization system that leverages role-based claims to secure your application's endpoints and resources.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddAppPermissionAuthorization<TKey>(this IServiceCollection services) where TKey : IEquatable<TKey>
    {
        services.AddScoped<IAuthorizationHandler, PermissionHandler<TKey>>();
        services.AddAuthorizationCore(options =>
            {
                foreach (var roleClaim in Enum.GetValues<RoleClaimType>().Cast<RoleClaimType>())
                {
                    options.AddPolicy(roleClaim.ToString(), policy =>
                    {
                        policy.Requirements.Add(new PermissionRequirement(roleClaim));
                    });
                }
            });

        return services;
    }
}