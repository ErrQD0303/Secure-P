using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SecureP.Service.Abstraction;
using SecureP.Service.Abstraction.Entities;
using SecureP.Shared;
using SecureP.Shared.Configures;
using YamlDotNet.Core.Tokens;

namespace Secure_P_Backend.Controllers;

[ApiController]
[Route(AppConstants.AppController.TokenController.DefaultRoute)]
public class TokenController : ControllerBase
{
    private readonly ITokenService<string> _tokenService;
    private readonly ILogger<TokenController> _logger;
    private readonly JwtConfigures _jwtConfigures;

    public TokenController(ITokenService<string> tokenService, ILogger<TokenController> logger, IOptions<JwtConfigures> jwtConfigures)
    {
        _tokenService = tokenService;
        _logger = logger;
        _jwtConfigures = jwtConfigures.Value;
    }

    [HttpPost(AppConstants.AppController.TokenController.GenerateToken)]
    public async Task<IActionResult> GenerateToken([FromBody] TokenRequest tokenRequest)
    {
        _logger.LogInformation($"Generating Tokens for user {tokenRequest.Email ?? tokenRequest.Username}");
        var response = await GenerateTokenResponseAsync(tokenRequest);

        return Ok(response);
    }

    [HttpPost(AppConstants.AppController.TokenController.RefreshToken)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        _logger.LogInformation($"Refreshing Tokens for tokens {refreshTokenRequest.RefreshToken}");

        var (isValid, appUser) = await _tokenService.ValidateRefreshTokenAsync(refreshTokenRequest);

        if (!isValid)
        {
            return BadRequest("Invalid Refresh Token");
        }

        var tokenRequest = new TokenRequest
        {
            Email = appUser?.Email,
            Username = appUser?.UserName
        };

        var tokenResponse = await IdentityController.SetAccessCookies(tokenRequest, appUser, Response, _tokenService, _jwtConfigures);

        return Ok(tokenResponse);
    }

    private async Task<TokenResponse> GenerateTokenResponseAsync(TokenRequest tokenRequest)
    {
        return new TokenResponse
        {
            AccessToken = await _tokenService.GenerateAccessTokenAsync(tokenRequest),
            RefreshToken = await _tokenService.GenerateRefreshTokenAsync(tokenRequest),
            TokenType = "Bearer"
        };
    }
}