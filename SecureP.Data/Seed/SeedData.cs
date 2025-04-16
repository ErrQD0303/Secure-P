using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SecureP.Identity.Models;
using SecureP.Identity.Models.Enum;
using SecureP.Shared;

namespace SecureP.Data.Seed;

public static class SeedData<TKey> where TKey : IEquatable<TKey>
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider, string defaultPW = "")
    {
        using var context = new AppDbContext<TKey>(serviceProvider.GetRequiredService<DbContextOptions<AppDbContext<TKey>>>());

        var adminId = await EnsureUserAsync(serviceProvider, AppConstants.DefaultAdminEmail, string.IsNullOrEmpty(defaultPW) ? AppConstants.DefaultAdminPassword : defaultPW);
        await EnsureRoleAsync(serviceProvider, adminId, RoleClaimType.Administrator.ToString(), RoleClaimType.Administrator, context);

        var normalUserId = await EnsureUserAsync(serviceProvider, AppConstants.DefaultNormalUserEmail, string.IsNullOrEmpty(defaultPW) ? AppConstants.DefaultNormalUserPassword : defaultPW);
        await EnsureRoleAsync(serviceProvider, normalUserId, RoleClaimType.NormalUser.ToString(), RoleClaimType.NormalUser, context);

        await SeedDBAsync(context);
    }

    public static async Task SeedDBAsync(AppDbContext<TKey> context)
    {
        // Add your seed data here. For example:
        var roles = await context.Roles.ToListAsync();
        var roleIds = roles.Select(r => r.Id).ToList();
        var existingUsers = await context.Users.Where(u => u.UserRoles == null || u.UserRoles.Count == 0 || u.UserRoles.Any(ur => !roleIds.Contains(ur.RoleId))).ToListAsync();

        foreach (var user in existingUsers)
        {
            var userRoles = await context.UserRoles.Where(ur => ur.UserId.Equals(user.Id)).ToListAsync();
            userRoles ??= [];
            if (userRoles.Count == 0)
            {
                var roleId = await context.Roles.Where(r => r.Name == RoleClaimType.NormalUser.ToString()).Select(r => r.Id).FirstOrDefaultAsync();

                if (roleId is null || default(TKey)?.Equals(roleId) == true)
                {
                    throw new Exception($"Role {RoleClaimType.NormalUser} not found.");
                }

                var roleClaim = new AppUserRole<TKey>
                {
                    UserId = user.Id,
                    RoleId = roleId, // Set the appropriate role ID here
                };
                context.UserRoles.Add(roleClaim);
                await context.SaveChangesAsync();
            }
        }
    }

    private static async Task<IdentityResult> EnsureRoleAsync(IServiceProvider serviceProvider, TKey uid, string roleName, RoleClaimType roleClaimType, AppDbContext<TKey> context)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole<TKey>>>();
        IdentityResult IR;

        var newRoleClaim = new AppRoleClaim<TKey>
        {
            ClaimType = roleClaimType.ToString(),
            ClaimValue = roleClaimType,
        };

        var existedRole = await context.Roles
            .Include(r => r.RoleClaims)
            .FirstOrDefaultAsync(r => r.Name == roleName);

        if (existedRole is not null)
        {
            var previousRoleClaims = existedRole.RoleClaims;
            existedRole.RoleClaims?.Clear();
            if (previousRoleClaims is not null)
            {
                context.RoleClaims.RemoveRange(previousRoleClaims);
                await context.SaveChangesAsync();
            }
        }
        else
        {
            existedRole = new AppRole<TKey>(roleName)
            {
                Id = typeof(TKey) switch
                {
                    Type t when t == typeof(string) => (TKey)(object)Guid.NewGuid().ToString(),
                    Type t when t == typeof(Guid) => (TKey)(object)Guid.NewGuid(),
                    _ => throw new NotSupportedException($"Type {typeof(TKey)} is not supported."),
                },
            };
            IR = await roleManager.CreateAsync(existedRole);
        }

        if (existedRole == null)
        {
            throw new Exception($"Role {roleName} could not be created.");
        }

        existedRole.RoleClaims ??= [];
        existedRole.RoleClaims.Add(newRoleClaim);
        IR = await context.SaveChangesAsync() > 0 ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = $"Role {roleName} could not be updated." });

        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser<TKey>>>();

        var user = await userManager.FindByIdAsync(uid.ToString() ?? string.Empty) ?? throw new Exception($"User with ID {uid} not found.");
        IR = await userManager.AddToRoleAsync(user, roleName);

        return IR;
    }

    private static async Task<TKey> EnsureUserAsync(IServiceProvider serviceProvider, string email, string password)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser<TKey>>>();

        var user = await userManager.FindByNameAsync(email);
        if (user == null)
        {
            user = new AppUser<TKey>
            {
                Id = typeof(TKey) switch
                {
                    Type t when t == typeof(string) => (TKey)(object)Guid.NewGuid().ToString(),
                    Type t when t == typeof(Guid) => (TKey)(object)Guid.NewGuid(),
                    _ => throw new NotSupportedException($"Type {typeof(TKey)} is not supported."),
                },
                Email = email,
                UserName = email,
                AddressLine1 = "Default Address",
                City = "Default City",
                Country = "Default Country",
                DayOfBirth = DateTime.UtcNow,
                FullName = "email",
                PostCode = "00000",
                EmailConfirmed = true,
            };
            await userManager.CreateAsync(user, password);
        }

        if (user == null)
        {
            throw new Exception($"User {email} could not be created because the password is not strong enough.");
        }

        return user.Id;
    }
}