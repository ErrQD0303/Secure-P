using SecureP.Service.Abstraction.Entities;

namespace SecureP.Service.Abstraction;

public interface ITokenService
{
    string GenerateAccessToken(TokenRequest tokenRequest);
    string GenerateRefreshToken(RefreshTokenRequest tokenRequest);
    bool ValidateAccessToken(string accessToken);  // Validate the actual token
    bool ValidateRefreshToken(string refreshToken);  // Validate the actual token
}