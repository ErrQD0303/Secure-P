using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SecureP.Identity.Models;
using SecureP.Identity.Models.Enum;
using SecureP.Shared;

namespace SecureP.Data.Seed;

/// <summary>
/// A static class responsible for seeding initial data into the application's database, such as default users and roles. This class ensures that essential data is present when the application is first run, allowing for proper authentication and authorization setup. The generic type parameter TKey allows for flexibility in the type of primary keys used for users and roles (e.g., string, Guid). The InitializeAsync method is the entry point for seeding data, which can be called during application startup to populate the database with necessary default values. The SeedDBAsync method can be customized to add additional seed data as needed. The EnsureUserAsync and EnsureRoleAsync methods handle the creation of default users and roles, ensuring that they exist in the database with the appropriate claims and permissions.
/// </summary>
/// <typeparam name="TKey">Type of your primary key</typeparam>
public static class SeedData<TKey> where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Initializes the database with default users and roles. This method is typically called during application startup to ensure that essential data is present in the database. It creates a default administrator user and a normal user, assigning them the appropriate roles and claims. The method also calls SeedDBAsync to allow for additional seeding of data as needed. The default password can be overridden by providing a value through configuration, ensuring that the seeded users have a secure password when the application is first run.
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="defaultPW"></param>
    /// <returns></returns>
    public static async Task InitializeAsync(IServiceProvider serviceProvider, string defaultPW = "")
    {
        using var context = new AppDbContext<TKey>(serviceProvider.GetRequiredService<DbContextOptions<AppDbContext<TKey>>>());

        var adminId = await EnsureUserAsync(serviceProvider, AppConstants.DefaultAdminEmail, string.IsNullOrEmpty(defaultPW) ? AppConstants.DefaultAdminPassword : defaultPW);
        await EnsureRoleAsync(serviceProvider, adminId, RoleClaimType.Administrator.ToString(), RoleClaimType.Administrator, context);

        var normalUserId = await EnsureUserAsync(serviceProvider, AppConstants.DefaultNormalUserEmail, string.IsNullOrEmpty(defaultPW) ? AppConstants.DefaultNormalUserPassword : defaultPW);
        await EnsureRoleAsync(serviceProvider, normalUserId, RoleClaimType.NormalUser.ToString(), RoleClaimType.NormalUser, context);

        await SeedDBAsync(context);
    }

    /// <summary>
    /// Seeds additional data into the database. This method can be customized to add any necessary seed data beyond the default users and roles created in the InitializeAsync method. It receives an instance of the application's DbContext, allowing for direct interaction with the database to add entities as needed. This method is called after the default users and roles have been created, ensuring that any additional data can be associated with those entities if necessary. The implementation of this method can be tailored to fit the specific requirements of the application, such as seeding default products, categories, or other domain-specific data.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
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

    /// <summary>
    /// Ensures that a role with the specified name exists in the database, and if not, creates it. Additionally, it assigns the specified role claim to the role and associates the role with the user identified by the provided user ID. This method is crucial for setting up default roles and permissions for users during the seeding process. It checks for the existence of the role, updates its claims if it already exists, or creates a new role if it does not. Finally, it adds the user to the role, ensuring that they have the appropriate permissions based on their assigned role claim.
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="uid"></param>
    /// <param name="roleName"></param>
    /// <param name="roleClaimType"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="Exception"></exception>
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