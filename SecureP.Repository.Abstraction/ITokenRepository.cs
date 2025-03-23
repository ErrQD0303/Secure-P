namespace SecureP.Repository.Abstraction;

public interface ITokenRepository<TKey> where TKey : IEquatable<TKey>
{
    Task<bool> AddTokenAsync(string token, TKey userId, TokenType tokenType, DateTime expiryDate, string loginProvider);
    Task<bool> ValidateTokenAsync(string token, TKey userId, TokenType tokenType, string loginProvider);
    Task<bool> RemoveTokenAsync(string token, TKey userId, TokenType tokenType, string loginProvider);
}

public enum TokenType
{
    AccessToken,
    RefreshToken,
    OTP
}