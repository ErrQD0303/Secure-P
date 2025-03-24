using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SecureP.Identity.Models;
using SecureP.Service.Abstraction;
using SecureP.Service.Abstraction.Entities;
using SecureP.Service.Abstraction.Exceptions;
using SecureP.Shared;
using SecureP.Shared.Configures;
using SecureP.Shared.Mappers;

namespace Secure_P_Backend.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class IdentityController : ControllerBase
{
    private readonly ILogger<IdentityController> _logger;
    private readonly IUserService<string> _userService;
    private readonly ITokenService<string> _tokenService;
    private readonly JwtConfigures _jwtConfigures;
    private readonly IEmailService _emailService;

    public IdentityController(ILogger<IdentityController> logger, IUserService<string> userService, ITokenService<string> tokenService, IOptions<JwtConfigures> jwtConfigures, IEmailService emailService)
    {
        _logger = logger;
        _userService = userService;
        _tokenService = tokenService;
        _jwtConfigures = jwtConfigures.Value;
        _emailService = emailService;
    }

    [AllowAnonymous]
    [HttpPost("user/register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        // Register user
        AppUser<string>? user = null;
        try
        {
            user = await _userService.RegisterAsync(registerRequest);
        }
        catch (UserRegisterException ex)
        {
            var passwordErrors = ex.IdentityErrors
                .Where(e => e.Code.StartsWith("Password"))
                .Select(e => e.Description)
                .Aggregate(string.Empty, (acc, desc) => acc + "\ndesc");
            var normalErrors = ex.IdentityErrors.Where(e => !e.Code.StartsWith("Password")).ToDictionary(e => e.Code, e => e.Description);
            var identityErrors = new Dictionary<string, string>(normalErrors)
            {
                { "Password", passwordErrors }
            };

            return BadRequest(new UserRegisterResponse<string>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Success = "false",
                Message = AppResponses.UserRegisterResponses.UserRegistrationFailed,
                Errors = identityErrors.Union(AppResponseErrors.UserRegisterErrors.UserRegistrationFailed).ToDictionary(e => e.Key, e => e.Value)
            });
        }

        if (user == null)
        {
            return BadRequest(new UserRegisterResponse<string>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Success = "false",
                Message = AppResponses.UserRegisterResponses.UserRegistrationFailed,
                Errors = AppResponseErrors.UserRegisterErrors.UserRegistrationFailed
            });
        }

        var userDto = new UserRegisterResponse<string>
        {
            StatusCode = StatusCodes.Status201Created,
            Success = "true",
            Message = AppResponses.UserRegisterResponses.UserRegistered,
            User = user.ToUserRegisterResponseAppUser()
        };

        return CreatedAtAction(nameof(GetUserInfo), new { id = user.Id }, userDto);
    }

    [HttpPost("user/logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return NotFound(new LogoutResponse<string>
            {
                StatusCode = StatusCodes.Status404NotFound,
                Success = "false",
                Message = AppResponses.UserLogoutResponses.UserNotFound,
            });
        }

        var user = await _userService.GetUserByIdAsync(userId);

        if (user == null)
        {
            return NotFound(new LogoutResponse<string>
            {
                StatusCode = StatusCodes.Status404NotFound,
                Success = "false",
                Message = AppResponses.UserLogoutResponses.UserNotFound,
            });
        }

        await RemoveAccessAndRefreshTokensAsync(Response, _tokenService, userId);

        return Ok(new LogoutResponse<string>
        {
            StatusCode = StatusCodes.Status200OK,
            Success = "true",
            Message = AppResponses.UserLogoutResponses.UserLoggedOut,
        });
    }

    public static async Task RemoveAccessAndRefreshTokensAsync(HttpResponse Response, ITokenService<string> tokenService, string userId)
    {
        await tokenService.InvalidateAccessTokenAsync(userId);
        await tokenService.InvalidateRefreshTokenAsync(userId);

        Response.Cookies.Append(AppConstants.CookieNames.AccessToken, string.Empty, new CookieOptions
        {
            HttpOnly = false,
            Secure = true, // Allow cookie to be sent over HTTP // Set to true for production
            SameSite = SameSiteMode.None, // Adjusted for frontend compatibility // Set to Strict for production
            Expires = DateTime.UtcNow,
            Path = "/",
            Domain = "localhost"
        });

        Response.Cookies.Append(AppConstants.CookieNames.RefreshToken, string.Empty, new CookieOptions
        {
            HttpOnly = false,
            Secure = true, // Allow cookie to be sent over HTTP // Set to true for production
            SameSite = SameSiteMode.None, // Adjusted for frontend compatibility // Set to Strict for production
            Expires = DateTime.UtcNow,
            Path = "/",
            Domain = "localhost"
        });
    }

    [HttpGet("user/getInfo")]
    public async Task<IActionResult> GetUserInfo()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return NotFound(new GetUserInfoResponse<string>
            {
                StatusCode = StatusCodes.Status404NotFound,
                Success = "false",
                Message = AppResponses.GetUserInfoResponses.UserNotFound,
            });
        }

        var user = await _userService.GetUserByIdAsync(userId);

        if (user == null)
        {
            return NotFound(new GetUserInfoResponse<string>
            {
                StatusCode = StatusCodes.Status404NotFound,
                Success = "false",
                Message = AppResponses.GetUserInfoResponses.UserNotFound,
            });
        }

        return Ok(new GetUserInfoResponse<string>
        {
            StatusCode = StatusCodes.Status200OK,
            Success = "true",
            Message = AppResponses.GetUserInfoResponses.UserFound,
            User = user.ToGetUserInfoResponseAppUser()
        });
    }

    [AllowAnonymous]
    [HttpPost("user/login")]
    public async Task<IActionResult> Login([FromRoute] LoginType loginType)
    {
        // Login user
        Request.EnableBuffering();

        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();
        Request.Body.Position = 0;

        var requestData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(body);

        var (loginResult, user) = loginType switch
        {
            LoginType.Email => await _userService.LoginByEmailAsync(new LoginByEmailRequest
            {
                Email = requestData?.email ?? string.Empty,
                Password = requestData?.password ?? string.Empty
            }),
            LoginType.Username => await _userService.LoginByUsernameAsync(new LoginByUsernameRequest
            {
                Username = requestData?.username ?? string.Empty,
                Password = requestData?.password ?? string.Empty
            }),
            _ => (false, null)
        };

        _logger.LogInformation($"User: {user?.Email ?? user?.UserName ?? user?.PhoneNumber ?? string.Empty} logged in.");

        if (!loginResult)
        {
            return Unauthorized(new LoginResponse<string>
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Success = "false",
                Message = AppResponses.UserLoginResponses.UserLoginFailed,
                Errors = AppResponseErrors.UserLoginErrors.UserLoginFailed
            });
        }

        if (!string.IsNullOrWhiteSpace(user?.Email))
        {
            Response.Cookies.Append(AppConstants.OTPConstant.TemporaryCookieName, user.Email, new CookieOptions
            {
                HttpOnly = false,
                Secure = true, // Allow cookie to be sent over HTTP // Set to true for production
                SameSite = SameSiteMode.None, // Adjusted for frontend compatibility // Set to Strict for production
                Expires = DateTime.UtcNow.AddMinutes(AppConstants.OTPConstant.ExpiryMinute),
                Path = "/",
                Domain = "localhost" // Ensure this matches the domain used when setting the cookie
            });

            var otp = await _tokenService.GenerateOTPAsync(user.Email);

            _ = _emailService.SendOTPEmailAsync(user.Email, otp);
        }

        return Ok(new LoginResponse<string>
        {
            StatusCode = StatusCodes.Status200OK,
            Success = "true",
            Message = AppResponses.UserLoginResponses.UserLoggedInWaitForOTP,
        });
    }

    [AllowAnonymous]
    [HttpPost("user/otp-login")]
    public async Task<IActionResult> OTPLogin([FromBody] OTPLoginRequest request)
    {
        if (!await _tokenService.ValidateOTPAsync(request.Email, request.OTP))
        {
            return Unauthorized(new LoginResponse<string>
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Success = "false",
                Message = AppResponses.UserLoginResponses.UserLoginFailed,
            });
        }

        var user = await _userService.GetUserByEmailAsync(request.Email);
        if (user == null)
        {
            return NotFound(new LoginResponse<string>
            {
                StatusCode = StatusCodes.Status404NotFound,
                Success = "false",
                Message = AppResponses.GetUserInfoResponses.UserNotFound,
            });
        }

        Response.Cookies.Append(AppConstants.OTPConstant.TemporaryCookieName, user.Email!, new CookieOptions
        {
            HttpOnly = false,
            Secure = true, // Allow cookie to be sent over HTTP // Set to true for production
            SameSite = SameSiteMode.None, // Adjusted for frontend compatibility // Set to Strict for production
            Expires = DateTime.UtcNow,
            Path = "/",
            Domain = "localhost" // Ensure this matches the domain used when setting the cookie
        });

        _ = await SetAccessCookies(request, user, Response, _tokenService, _jwtConfigures);

        return Ok(new LoginResponse<string>
        {
            StatusCode = StatusCodes.Status200OK,
            Success = "true",
            Message = AppResponses.UserLoginResponses.UserLoggedIn,
            User = user.ToLoginResponseAppUser()
        });
    }

    public static async Task<TokenResponse> SetAccessCookies(dynamic? requestData, AppUser<string>? user, HttpResponse Response, ITokenService<string> tokenService, JwtConfigures jwtConfigures)
    {
        var tokenResponse = new TokenResponse
        {
            AccessToken = await tokenService.GenerateAccessTokenAsync(new TokenRequest
            {
                Email = user?.Email ?? string.Empty,
            }),
            RefreshToken = await tokenService.GenerateRefreshTokenAsync(new TokenRequest
            {
                Email = user?.Email ?? string.Empty,
            }),
            TokenType = "Bearer"
        };

        Response.Cookies.Append("access_token", tokenResponse.AccessToken, new CookieOptions
        {
            HttpOnly = false,
            Secure = true, // Allow cookie to be sent over HTTP // Set to true for production
            SameSite = SameSiteMode.None, // Adjusted for frontend compatibility // Set to Strict for production
            Expires = DateTime.UtcNow.AddSeconds(jwtConfigures.ExpirySeconds),
            Path = "/",
            Domain = "localhost"
        });

        Response.Cookies.Append("refresh_token", tokenResponse.RefreshToken, new CookieOptions
        {
            HttpOnly = false,
            Secure = true, // Allow cookie to be sent over HTTP // Set to true for production
            SameSite = SameSiteMode.None, // Adjusted for frontend compatibility // Set to Strict for production
            Expires = DateTime.UtcNow.AddSeconds(jwtConfigures.RefreshExpirySeconds),
            Path = "/",
            Domain = "localhost"
        });

        return tokenResponse;
    }

    [HttpGet("get-user/{id}")]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUser([FromRoute] string id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user.ToGetUserDto());
    }

    [HttpGet("get-users")]
    // [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers()
    {
        var users = (await _userService.GetUsersAsync())
            .Select(u => u.ToGetUserDto());

        return Ok(users);
    }
}

public enum LoginType
{
    Email = 0,
    Username,
    PhoneNumber
}