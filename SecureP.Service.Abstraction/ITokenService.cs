using SecureP.Service.Abstraction.Entities;

namespace SecureP.Service.Abstraction;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(TokenRequest tokenRequest);
    Task<string> GenerateRefreshTokenAsync(TokenRequest tokenRequest);
    Task<bool> ValidateAccessTokenAsync(string accessToken, string username);  // Validate the actual token
    Task<bool> ValidateRefreshTokenAsync(RefreshTokenRequest request);  // Validate the actual token
    Task<string> GenerateOTPAsync(string email);
    Task<bool> ValidateOTPAsync(string email, string otp);
}