using Microsoft.EntityFrameworkCore;
using SecureP.Data;
using SecureP.Identity.Models;
using SecureP.Repository.Abstraction;
using System.Linq.Expressions;

namespace SecureP.Repository.UserRepository;

public class UserRepository<TKey>(AppDbContext<TKey> context) : IUserRepository<TKey> where TKey : IEquatable<TKey>
{
    private readonly AppDbContext<TKey> _context = context;

    public Task<AppUser<TKey>?> FindByIdAsync(TKey id, bool includeUserRoles = false, bool includeUserTokens = false, bool includeUserLogins = false)
    {
        if (id is null) return Task.FromResult<AppUser<TKey>?>(null);

        return FindSingleUserAsync(u => u.Id != null && u.Id.Equals(id), includeUserRoles, includeUserTokens, includeUserLogins);
    }

    public Task<AppUser<TKey>?> FindByEmailAsync(string email, bool includeUserRoles = false, bool includeUserTokens = false, bool includeUserLogins = false)
    {
        if (string.IsNullOrWhiteSpace(email)) return Task.FromResult<AppUser<TKey>?>(null);

        var normalizedEmail = email.ToUpperInvariant();
        return FindSingleUserAsync(u => u.NormalizedEmail != null && u.NormalizedEmail == normalizedEmail, includeUserRoles, includeUserTokens, includeUserLogins);
    }

    public Task<AppUser<TKey>?> FindByUsernameAsync(string username, bool includeUserRoles = false, bool includeUserTokens = false, bool includeUserLogins = false)
    {
        if (string.IsNullOrWhiteSpace(username)) return Task.FromResult<AppUser<TKey>?>(null);

        var normalizedUsername = username.ToUpperInvariant();
        return FindSingleUserAsync(u => u.NormalizedUserName != null && u.NormalizedUserName == normalizedUsername, includeUserRoles, includeUserTokens, includeUserLogins);
    }

    public Task<AppUser<TKey>?> FindByPhoneAsync(string phone, bool includeUserRoles = false, bool includeUserTokens = false, bool includeUserLogins = false)
    {
        if (string.IsNullOrWhiteSpace(phone)) return Task.FromResult<AppUser<TKey>?>(null);

        var normalizedPhone = phone.Trim();
        return FindSingleUserAsync(u => u.PhoneNumber != null && u.PhoneNumber == normalizedPhone, includeUserRoles, includeUserTokens, includeUserLogins);
    }

    private static IQueryable<AppUser<TKey>> UsersWithUserRoles(IQueryable<AppUser<TKey>> user)
    {
        return user.Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.RoleClaims);
    }

    private static IQueryable<AppUser<TKey>> UsersWithUserTokens(IQueryable<AppUser<TKey>> user)
    {
        return user.Include(u => u.UserTokens);
    }

    private static IQueryable<AppUser<TKey>> UsersWithUserLogins(IQueryable<AppUser<TKey>> user)
    {
        return user.Include(u => u.UserLogins);
    }

    private Task<AppUser<TKey>?> FindSingleUserAsync(Expression<Func<AppUser<TKey>, bool>> predicate, bool includeUserRoles = false, bool includeUserTokens = false, bool includeUserLogins = false)
    {
        var users = _context.Users.AsQueryable().AsSplitQuery();

        if (includeUserRoles)
        {
            users = UsersWithUserRoles(users);
        }
        if (includeUserTokens)
        {
            users = UsersWithUserTokens(users);
        }
        if (includeUserLogins)
        {
            users = UsersWithUserLogins(users);
        }

        return users.FirstOrDefaultAsync(predicate);
    }

    public Task<bool> SaveChangesAsync()
    {
        return Task.FromResult(_context.SaveChanges() > 0);
    }
}
