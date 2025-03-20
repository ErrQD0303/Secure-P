using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Secure_P_Backend.Models;
using SecureP.Service.Abstraction;
using SecureP.Service.Abstraction.Entities;

namespace Secure_P_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TokenControllers : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly ILogger<TokenControllers> _logger;

    public TokenControllers(ITokenService tokenService, ILogger<TokenControllers> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost("token")]
    public async Task<IActionResult> GenerateToken([FromBody] TokenRequest tokenRequest)
    {
        var response = await GenerateTokenResponseAsync(tokenRequest);

        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
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