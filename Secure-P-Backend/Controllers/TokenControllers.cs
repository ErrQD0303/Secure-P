using Microsoft.AspNetCore.Mvc;

namespace Secure_P_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TokenControllers : ControllerBase
{
    /* private readonly ITokenService _tokenService;
    private readonly ILogger<TokenControllers> _logger;

    public TokenControllers(ITokenService tokenService, ILogger<TokenControllers> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost("generate")]
    public IActionResult GenerateToken([FromBody] TokenRequest tokenRequest)
    {
        var token = _tokenService.GenerateToken(tokenRequest);
        return Ok(token);
    }

    [HttpPost("validate")]
    public IActionResult ValidateToken([FromBody] TokenRequest tokenRequest)
    {
        var isValid = _tokenService.ValidateToken(tokenRequest);
        return Ok(isValid);
    } */
}