using SecureP.Identity.Models;

namespace SecureP.Repository.Abstraction;

public interface ITokenRepository<TKey> where TKey : IEquatable<TKey>
{
    Task AddTokenAsync(string token, AppUser<TKey> user, TokenType tokenType, DateTime expiryDate);
    Task<bool> ValidateTokenAsync(string token, AppUser<TKey> user, TokenType tokenType);
    Task<bool> RemoveTokenAsync(string token, TKey userId, TokenType tokenType, string loginProvider);
    Task RemoveTokenAsync(TKey userId, TokenType tokenType);
    Task RemoveTokenAsync(AppUser<TKey> user, TokenType tokenType);
    Task<AppUser<TKey>?> GetUserByTokenAsync(string token, TokenType tokenType);
    Task RemoveUserTokenAsync(AppUser<TKey> user, AppUserToken<TKey> existingToken);
    Task<bool> SaveChangesAsync();
}

public enum TokenType
{
    AccessToken,
    RefreshToken,
    OTP
}