using SecureP.Identity.Models;

namespace SecureP.Repository.Abstraction;

public interface IUserRepository<TKey> where TKey : IEquatable<TKey>
{
    Task<AppUser<TKey>?> FindByEmailAsync(string email, bool includeUserRoles = false, bool includeUserTokens = false, bool includeUserLogins = false);
    Task<AppUser<TKey>?> FindByUsernameAsync(string email, bool includeUserRoles = false, bool includeUserTokens = false, bool includeUserLogins = false);
    Task<AppUser<TKey>?> FindByPhoneAsync(string email, bool includeUserRoles = false, bool includeUserTokens = false, bool includeUserLogins = false);
}