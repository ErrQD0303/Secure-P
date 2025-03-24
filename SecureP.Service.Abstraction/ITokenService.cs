using SecureP.Identity.Models;
using SecureP.Service.Abstraction.Entities;

namespace SecureP.Service.Abstraction;

public interface ITokenService<TKey> where TKey : IEquatable<TKey>
{
    Task<string> GenerateAccessTokenAsync(TokenRequest tokenRequest);
    Task<string> GenerateRefreshTokenAsync(TokenRequest tokenRequest);
    Task<bool> ValidateAccessTokenAsync(string accessToken, string username);  // Validate the actual token
    Task<(bool isValid, AppUser<TKey>? appUser)> ValidateRefreshTokenAsync(RefreshTokenRequest request);
    Task<string> GenerateOTPAsync(string email);
    Task<bool> ValidateOTPAsync(string email, string otp);
    Task InvalidateRefreshTokenAsync(TKey userId);
    Task InvalidateAccessTokenAsync(TKey userId);
}