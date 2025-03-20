namespace SecureP.Repository.Abstraction;

public interface ITokenRepository
{
    Task<bool> AddTokenAsync(string token, string userId, TokenType tokenType, DateTime expiryDate, string loginProvider);
    Task<bool> ValidateTokenAsync(string token, string userId, TokenType tokenType, string loginProvider);
    Task<bool> RemoveTokenAsync(string token, string userId, TokenType tokenType, string loginProvider);
}

public enum TokenType
{
    AccessToken,
    RefreshToken
}