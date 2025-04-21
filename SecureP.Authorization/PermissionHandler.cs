using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecureP.Cache.Abstraction;
using SecureP.Identity.Models;
using SecureP.Identity.Models.Enum;
using SecureP.Shared;

namespace SecureP.Authorization;

public class PermissionHandler<TKey> : AuthorizationHandler<PermissionRequirement> where TKey : IEquatable<TKey>
{
    public RoleManager<AppRole<TKey>> RoleManager { get; private set; }
    public UserManager<AppUser<TKey>> UserManager { get; private set; }
    public ICacheManager CacheManager { get; private set; }

    public PermissionHandler(RoleManager<AppRole<TKey>> roleManager, UserManager<AppUser<TKey>> userManager, ICacheManager cacheManager)
    {
        RoleManager = roleManager;
        UserManager = userManager;
        CacheManager = cacheManager;
    }

    private static string GetCacheKey(string userId) => AppConstants.CachePermission.GetRedisKey(userId);

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            context.Fail(new AuthorizationFailureReason(this, "User ID not found in claims."));
            return;
        }

        if (await UserHasPermissionAsync(userId, requirement))
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail(new AuthorizationFailureReason(this, requirement.Permission.ToString()));
    }

    private async Task<bool> UserHasPermissionAsync(string userId, PermissionRequirement requirement)
    {
        var user = await UserManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        var requestUserRoles = await CacheManager.GetAsync<List<AppRole<TKey>>>(GetCacheKey(userId));
        if (requestUserRoles is null || requestUserRoles.Count == 0)
        {
            var userRolesName = await UserManager.GetRolesAsync(user);

            if (userRolesName is null || userRolesName.Count == 0)
            {
                return false;
            }

            requestUserRoles = await RoleManager.Roles
                .Where(role => userRolesName.Contains(role.Name ?? string.Empty))
                .Where(role => role.UserRoles.Any(ur => ur.UserId.Equals(user.Id)))
                .Include(role => role.RoleClaims)
                .ToListAsync() ?? [];

            await CacheManager.SetAsync(GetCacheKey(userId), requestUserRoles, TimeSpan.FromSeconds(AppConstants.CacheOptions.AbsoluteExpirationRelativeToNow));
        }

        if (requestUserRoles.SelectMany(role => role.RoleClaims)
            .Any(roleClaim => roleClaim.ClaimValue == RoleClaimType.Administrator || (requirement.Permission & roleClaim.ClaimValue) == requirement.Permission))
        {
            return true;
        }

        return false;
    }
}