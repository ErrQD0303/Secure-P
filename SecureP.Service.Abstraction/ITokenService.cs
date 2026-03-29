using SecureP.Identity.Models;
using SecureP.Service.Abstraction.Entities;
using SecureP.Service.Abstraction.Results;

namespace SecureP.Service.Abstraction;

public interface ITokenService<TKey> where TKey : IEquatable<TKey>
{
    Task<string> GenerateAccessTokenAsync(TokenRequest tokenRequest);
    Task<string> GenerateAccessTokenAsync(AppUser<TKey> user);
    Task<string> GenerateRefreshTokenAsync(TokenRequest tokenRequest);
    Task<string> GenerateRefreshTokenAsync(AppUser<TKey> user);
    Task<(string AccessToken, string RefreshToken)> GenerateAccessAndRefreshTokensAsync(AppUser<TKey> user);
    Task<bool> ValidateAccessTokenAsync(string accessToken, string username);  // Validate the actual token
    Task<(bool isValid, AppUser<TKey>? appUser)> ValidateRefreshTokenAsync(RefreshTokenRequest request);
    Task<string> GenerateOTPAsync(AppUser<TKey> user);
    Task<Result<AppUser<TKey>>> ValidateOTPAsync(string email, string otp);
    Task InvalidateRefreshTokenAsync(TKey userId);
    Task InvalidateAccessTokenAsync(TKey userId);
    Task InvalidateAccessAndRefreshTokensAsync(TKey userId);
}