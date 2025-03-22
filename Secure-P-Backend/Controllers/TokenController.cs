using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecureP.Service.Abstraction;
using SecureP.Service.Abstraction.Entities;

namespace Secure_P_Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class TokenController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<TokenController> _logger;

    public TokenController(ITokenService tokenService, ILogger<TokenController> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost("token")]
    public async Task<IActionResult> GenerateToken([FromBody] TokenRequest tokenRequest)
    {
        _logger.LogInformation($"Generating Tokens for user {tokenRequest.Email ?? tokenRequest.Username}");
        var response = await GenerateTokenResponseAsync(tokenRequest);

        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        _logger.LogInformation($"Refreshing Tokens for tokens {refreshTokenRequest.RefreshToken}");
        if (!await _tokenService.ValidateRefreshTokenAsync(refreshTokenRequest))
        {
            return BadRequest("Invalid Refresh Token");
        }

        var response = await GenerateTokenResponseAsync(refreshTokenRequest);

        return Ok(response);
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