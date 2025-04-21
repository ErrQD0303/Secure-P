
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using SecureP.Identity.Models.Enum;

namespace SecureP.Authorization;

public static class AuthorizationExtensions
{
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