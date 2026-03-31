using SecureP.Service.Abstraction.Results;

namespace Secure_P_Backend.Controllers;

[ApiController]
[Route(AppConstants.AppController.TokenController.DefaultRoute)]
public class TokenController : ControllerBase
{
    private readonly ITokenService<string> _tokenService;
    private readonly IUserService<string> _userService;
    private readonly ILogger<TokenController> _logger;
    private readonly JwtConfigures _jwtConfigures;

    public TokenController(ITokenService<string> tokenService, ILogger<TokenController> logger, IOptions<JwtConfigures> jwtConfigures, IUserService<string> userService)
    {
        _tokenService = tokenService;
        _logger = logger;
        _jwtConfigures = jwtConfigures.Value;
        _userService = userService;
    }

    [HttpPost(AppConstants.AppController.TokenController.GenerateToken)]
    public async Task<IActionResult> GenerateToken([FromBody] TokenRequest tokenRequest)
    {
        if (string.IsNullOrEmpty(tokenRequest.Email) && string.IsNullOrEmpty(tokenRequest.Username))
        {
            return BadRequest(new GenerateTokenResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Success = false,
                Message = AppResponses.GenerateTokenResponses.MissingCredentials,
                Errors = AppResponseErrors.GenerateTokenErrors.MissingCredentials
            });
        }

        _logger.LogInformation("Generating Tokens for user {user}", tokenRequest.Email ?? tokenRequest.Username);
        Result<AppUser<string>?> result;
        if (tokenRequest.Email is not null)
        {
            result = await _userService.LoginByEmailAsync(new LoginByEmailRequest
            {
                Email = tokenRequest.Email,
                Password = tokenRequest.Password
            }, includeUserTokens: true, includeUserLogins: true, includeUserRoles: true);

            if (!result.IsSuccess || result.Value is null)
            {
                return NotFound(new GenerateTokenResponse
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Success = false,
                    Message = AppResponses.GenerateTokenResponses.InvalidEmailOrPassword,
                    Errors = result.Errors.ToDictionary(e => e.Code, e => (object)e.Description)
                });
            }
        }
        else
        {
            result = await _userService.LoginByUsernameAsync(new LoginByUsernameRequest
            {
                Username = tokenRequest.Username!,
                Password = tokenRequest.Password
            }, includeUserTokens: true, includeUserLogins: true, includeUserRoles: true);

            if (!result.IsSuccess || result.Value is null)
            {
                return NotFound(new GenerateTokenResponse
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Success = false,
                    Message = AppResponses.GenerateTokenResponses.InvalidUsernameOrPassword,
                    Errors = result.Errors.ToDictionary(e => e.Code, e => (object)e.Description)
                });
            }
        }

        var user = result.Value!;

        var (AccessToken, RefreshToken) = await _tokenService.GenerateAccessAndRefreshTokensAsync(user);

        var tokenResponseDto = new TokenResponseDto
        {
            AccessToken = AccessToken,
            RefreshToken = RefreshToken,
            TokenType = "Bearer"
        };

        return Ok(new GenerateTokenResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Success = true,
            Message = AppResponses.GenerateTokenResponses.TokensGeneratedSuccessfully,
            Tokens = tokenResponseDto
        });
    }

    [HttpPost(AppConstants.AppController.TokenController.RefreshToken)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
    {
        if (string.IsNullOrWhiteSpace(refreshTokenRequest.RefreshToken))
        {
            return BadRequest(new RefreshTokenResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Success = false,
                Message = AppResponses.RefreshTokenResponses.InvalidRefreshToken,
                Errors = AppResponseErrors.RefreshTokenErrors.InvalidRefreshToken
            });
        }

        _logger.LogInformation("Refreshing Tokens for tokens {@tokens}", refreshTokenRequest.RefreshToken);

        var result = await _tokenService.ValidateRefreshTokenAsync(refreshTokenRequest);
        var isValid = result.IsSuccess;

        if (!isValid)
        {
            return BadRequest(new RefreshTokenResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Success = false,
                Message = AppResponses.RefreshTokenResponses.InvalidRefreshToken,
                Errors = result.Errors.ToDictionary(e => e.Code, e => (object)e.Description)
            });
        }

        var appUser = result.Value!;

        var tokenRequest = new TokenRequest
        {
            Email = appUser?.Email,
            Username = appUser?.UserName
        };

        var tokenResponse = await IdentityController.SetAccessCookies(appUser!, Request, Response, _tokenService, _jwtConfigures);

        return Ok(tokenResponse);
    }
}