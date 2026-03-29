using SecureP.Identity.Models;

namespace SecureP.Repository.Abstraction;

public interface ITokenRepository<TKey> where TKey : IEquatable<TKey>
{
    Task<bool> AddTokenAsync(string token, AppUser<TKey> user, TokenType tokenType, DateTime expiryDate);
    Task<bool> ValidateTokenAsync(string token, AppUser<TKey> user, TokenType tokenType);
    Task<bool> RemoveTokenAsync(string token, TKey userId, TokenType tokenType, string loginProvider);
    Task<bool> RemoveTokenAsync(TKey userId, TokenType tokenType);
    Task<AppUser<TKey>?> GetUserByTokenAsync(string token, TokenType tokenType);
    Task RemoveUserTokenAsync(AppUser<TKey> user, AppUserToken<TKey> existingToken);
}

public enum TokenType
{
    AccessToken,
    RefreshToken,
    OTP
}