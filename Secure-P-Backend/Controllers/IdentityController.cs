using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SecureP.Identity.Models;
using SecureP.Service.Abstraction;
using SecureP.Service.Abstraction.Entities;
using SecureP.Service.UserService;
using SecureP.Shared.Configures;

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

    public IdentityController(ILogger<IdentityController> logger, IUserService<string> userService, ITokenService tokenService, IOptions<JwtConfigures> jwtConfigures)
    {
        _logger = logger;
        _userService = userService;
        _tokenService = tokenService;
        _jwtConfigures = jwtConfigures.Value;
    }

    [AllowAnonymous]
    [HttpPost("user/register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        // Register user
        var user = await _userService.RegisterAsync(registerRequest);

        if (user == null)
        {
            return BadRequest("Register failed");
        }

        var userDto = new
        {
            user.Id,
            user.UserName,
            user.Email,
            user.PhoneNumber,
            user.FullName,
            user.DayOfBirth,
            user.Country,
            user.City,
            user.AddressLine1,
            user.AddressLine2,
            user.PostCode,
            LicensePlates = user.UserLicensePlates.Select(lp => new
            {
                lp.LicensePlateNumber
            })
        };

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
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

        if (!loginResult)
        {
            return Unauthorized(new LoginResponse
            {
                StatusCode = 401,
                Success = "false",
                Message = "Login failed, Invalid email or password!",
                Errors = new Dictionary<string, string>
                {
                    { "summary", "Invalid email or password!" },
                }
            });
        }

        await SetAccessCookies(requestData, user);

        return Ok(new LoginResponse
        {
            StatusCode = 200,
            Success = "true",
            Message = "Login successful!",
            User = new LoginResponseAppUser<string>
            {
                Id = user?.Id!,
                Username = user?.UserName!,
                Email = user?.Email!,
                PhoneNumber = user?.PhoneNumber!,
                FullName = user?.FullName!,
                DayOfBirth = user!.DayOfBirth!,
                Country = user.Country,
                City = user.City,
                AddressLine1 = user.AddressLine1,
                AddressLine2 = user.AddressLine2,
                PostCode = user.PostCode,
                UserLicensePlates = user.UserLicensePlates
            }
        });
    }

    private async Task SetAccessCookies(dynamic? requestData, AppUser<string>? user)
    {
        Response.Cookies.Append("access_token", await _tokenService.GenerateAccessTokenAsync(new TokenRequest
        {
            Email = user?.Email ?? string.Empty,
            Password = requestData?.password ?? string.Empty,
        }), new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // Allow cookie to be sent over HTTP // Set to true for production
            SameSite = SameSiteMode.None, // Adjusted for frontend compatibility // Set to Strict for production
            Expires = DateTime.UtcNow.AddSeconds(_jwtConfigures.ExpirySeconds),
            Path = "/",
            Domain = "localhost"
        });

        Response.Cookies.Append("refresh_token", await _tokenService.GenerateRefreshTokenAsync(new TokenRequest
        {
            Email = user?.Email ?? string.Empty,
            Password = requestData?.password ?? string.Empty,
        }), new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // Allow cookie to be sent over HTTP // Set to true for production
            SameSite = SameSiteMode.None, // Adjusted for frontend compatibility // Set to Strict for production
            Expires = DateTime.UtcNow.AddSeconds(_jwtConfigures.RefreshExpirySeconds),
            Path = "/",
            Domain = "localhost"
        });
    }

    [HttpGet("get-user/{id}")]
    public async Task<IActionResult> GetUser([FromRoute] string id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        var returnedUser = new
        {
            user.Id,
            user.UserName,
            user.Email,
            user.EmailConfirmed,
            user.PhoneNumber,
            user.PhoneNumberConfirmed,
            user.DayOfBirth,
            user.Country,
            user.City,
            user.AddressLine1,
            user.AddressLine2,
            user.FullName,
            user.TwoFactorEnabled,
            user.LockoutEnabled,
            user.LockoutEnd,
            UserTokens = user.UserTokens.Select(t => new
            {
                t.LoginProvider,
                t.Name,
                t.Value,
                t.ExpiryDate
            }),
            LicensePlates = user.UserLicensePlates.Select(lp => new
            {
                lp.LicensePlateNumber,
            }),
            user.AccessFailedCount,
        };

        return Ok(returnedUser);
    }

    [HttpGet("get-users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = (await _userService.GetUsersAsync())
            .Select(u => new
            {
                u.Id,
                u.UserName,
                u.Email,
                u.EmailConfirmed,
                u.PhoneNumber,
                u.PhoneNumberConfirmed,
                u.DayOfBirth,
                u.Country,
                u.City,
                u.AddressLine1,
                u.AddressLine2,
                u.FullName,
                u.TwoFactorEnabled,
                u.LockoutEnabled,
                u.LockoutEnd,
                UserTokens = u.UserTokens.Select(t => new
                {
                    t.LoginProvider,
                    t.Name,
                    t.Value,
                    t.ExpiryDate
                }),
                LicensePlates = u.UserLicensePlates.Select(lp => new
                {
                    lp.LicensePlateNumber,
                }),
                u.AccessFailedCount,
            });

        return Ok(users);
    }
}

public enum LoginType
{
    Email = 0,
    Username,
    PhoneNumber
}