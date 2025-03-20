using Microsoft.Extensions.Logging;
using SecureP.Repository.Abstraction;
using SecureP.Service.Abstraction;
using SecureP.Service.Abstraction.Entities;

namespace SecureP.Service.TokenService;

public class TokenService : ITokenService
{
    private readonly ITokenRepository _tokenRepository;
    private readonly ILogger<TokenService> _logger;

    public TokenService(ITokenRepository tokenRepository, ILogger<TokenService> logger)
    {
        _tokenRepository = tokenRepository;
        _logger = logger;
    }

    public string GenerateAccessToken(TokenRequest tokenRequest)
    {
        _logger.LogInformation("Generating Access Token");
        throw new NotImplementedException();
    }

    public string GenerateRefreshToken(RefreshTokenRequest tokenRequest)
    {
        _logger.LogInformation("Generating Refresh Token");
        throw new NotImplementedException();
    }

    public bool ValidateAccessToken(string accessToken)
    {
        _logger.LogInformation("Validating Access Token");
        throw new NotImplementedException();
    }

    public bool ValidateRefreshToken(string refreshToken)
    {
        _logger.LogInformation("Validating Refresh Token");
        throw new NotImplementedException();
    }
}
