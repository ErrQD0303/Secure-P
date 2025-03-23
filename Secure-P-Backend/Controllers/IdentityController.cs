using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SecureP.Identity.Models;
using SecureP.Identity.Models.Dto;
using SecureP.Service.Abstraction;
using SecureP.Service.Abstraction.Entities;
using SecureP.Service.UserService;
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
    private readonly ITokenService _tokenService;
    private readonly JwtConfigures _jwtConfigures;
    private readonly IEmailService _emailService;

    public IdentityController(ILogger<IdentityController> logger, IUserService<string> userService, ITokenService tokenService, IOptions<JwtConfigures> jwtConfigures, IEmailService emailService)
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
        var user = await _userService.RegisterAsync(registerRequest);

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
            StatusCode = StatusCodes.Status200OK,
            Success = "true",
            Message = AppResponses.GetUserInfoResponses.UserFound,
            User = user.ToUserRegisterResponseAppUser()
        };

        return CreatedAtAction(nameof(GetUserInfo), new { id = user.Id }, userDto);
    }

    [HttpGet("user/getInfo")]
    public async Task<IActionResult> GetUserInfo()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return BadRequest("User not found");
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

            _emailService.SendOTPEmailAsync(user.Email, otp);
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

        await SetAccessCookies(request, user);

        return Ok(new LoginResponse<string>
        {
            StatusCode = StatusCodes.Status200OK,
            Success = "true",
            Message = AppResponses.UserLoginResponses.UserLoggedIn,
            User = user.ToLoginResponseAppUser()
        });
    }

    private async Task SetAccessCookies(dynamic? requestData, AppUser<string>? user)
    {
        Response.Cookies.Append("access_token", await _tokenService.GenerateAccessTokenAsync(new TokenRequest
        {
            Email = user?.Email ?? string.Empty,
        }), new CookieOptions
        {
            HttpOnly = false,
            Secure = true, // Allow cookie to be sent over HTTP // Set to true for production
            SameSite = SameSiteMode.None, // Adjusted for frontend compatibility // Set to Strict for production
            Expires = DateTime.UtcNow.AddSeconds(_jwtConfigures.ExpirySeconds),
            Path = "/",
            Domain = "localhost"
        });

        Response.Cookies.Append("refresh_token", await _tokenService.GenerateRefreshTokenAsync(new TokenRequest
        {
            Email = user?.Email ?? string.Empty,
        }), new CookieOptions
        {
            HttpOnly = false,
            Secure = true, // Allow cookie to be sent over HTTP // Set to true for production
            SameSite = SameSiteMode.None, // Adjusted for frontend compatibility // Set to Strict for production
            Expires = DateTime.UtcNow.AddSeconds(_jwtConfigures.RefreshExpirySeconds),
            Path = "/",
            Domain = "localhost"
        });
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