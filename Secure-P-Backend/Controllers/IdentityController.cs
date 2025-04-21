using SecureP.Identity.Models.Enum;
using SecureP.Identity.Models; // Ensure this namespace is imported

namespace Secure_P_Backend.Controllers;

[ApiController]
[Route(AppConstants.AppController.IdentityController.DefaultRoute)]
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
    [HttpPost(AppConstants.AppController.IdentityController.Register)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        _logger.LogInformation($"Registering user with email: {registerRequest.Email}");
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

    [HttpPost(AppConstants.AppController.IdentityController.Logout)]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        _logger.LogInformation($"Logging out user with email: {User.FindFirstValue(ClaimTypes.Email)}");
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

    [NonAction]
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

    [HttpGet(AppConstants.AppController.IdentityController.GetUserInfo)]
    [Authorize(Policy = AppPolicy.GetInfo)] // Ensure RoleClaimType is defined in SecureP.Identity.Models
    public async Task<IActionResult> GetUserInfo()
    {
        _logger.LogInformation($"Getting user info for user with email: {User.FindFirstValue(ClaimTypes.Email)}");
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
        var userRoles = await _userService.GetUserRolesAsync(userId);
        var userPermissions = await _userService.GetUserPermissionsAsync(userId);

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
            User = user.ToGetUserInfoResponseAppUser(userRoles, userPermissions)
        });
    }

    [AllowAnonymous]
    [HttpPost(AppConstants.AppController.IdentityController.Login)]
    public async Task<IActionResult> Login([FromRoute] LoginType loginType)
    {
        // Login user
        Request.EnableBuffering();

        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();
        Request.Body.Position = 0;

        var requestData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(body);

        _logger.LogInformation($"Logging in user: {requestData?.email ?? requestData?.username ?? string.Empty}");
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
    [HttpPost(AppConstants.AppController.IdentityController.OTPLogin)]
    public async Task<IActionResult> OTPLogin([FromBody] OTPLoginRequest request)
    {
        _logger.LogInformation($"Logging in user with email: {request.Email} using OTP");
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

    [NonAction]
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

    [AllowAnonymous]
    [HttpGet(AppConstants.AppController.IdentityController.ConfirmEmail)]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest request)
    {
        _logger.LogInformation($"Confirming email for user with email: {request.Email}");
        if (await _userService.ConfirmEmailAsync(request))
        {
            return Ok(new ConfirmEmailResponse<string>
            {
                StatusCode = StatusCodes.Status200OK,
                Success = "true",
                Message = AppResponses.EmailConfirmationResponses.EmailConfirmed,
            });
        }

        return BadRequest(new ConfirmEmailResponse<string>
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Success = "false",
            Message = AppResponses.EmailConfirmationResponses.EmailConfirmationFailed,
            Errors = AppResponseErrors.EmailConfirmationErrors.EmailConfirmationFailed
        });
    }

    [HttpPost(AppConstants.AppController.IdentityController.ResendEmailConfirmation)]
    [Authorize(Policy = AppPolicy.ResendEmailConfirmation)] // Ensure RoleClaimType is defined in SecureP.Identity.Models
    public async Task<IActionResult> ResendConfirmationEmail(ResendConfirmEmailRequest request)
    {
        _logger.LogInformation($"Resending confirmation email to user with email: {request.Email}");
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (email == null)
        {
            return NotFound(new ResendConfirmEmailResponse<string>
            {
                StatusCode = StatusCodes.Status404NotFound,
                Success = "false",
                Message = AppResponses.ResendEmailConfirmationResponses.UserNotFound,
            });
        }

        if (email != request.Email)
        {
            return BadRequest(new ResendConfirmEmailResponse<string>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Success = "false",
                Message = AppResponses.ResendEmailConfirmationResponses.EmailIsNotMatch,
                Errors = AppResponseErrors.ResendEmailConfirmationErrors.EmailIsNotMatch
            });
        }

        await _userService.ResendConfirmationEmailAsync(email);

        return Ok(new ResendConfirmEmailResponse<string>
        {
            StatusCode = StatusCodes.Status200OK,
            Success = "true",
            Message = AppResponses.ResendEmailConfirmationResponses.EmailSent,
        });
    }

    [HttpGet(AppConstants.AppController.IdentityController.AdminUser.GetUser)]
    [Authorize(Policy = AppPolicy.ReadUser)]
    public async Task<IActionResult> GetUser([FromRoute] string id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user.ToGetUserDto());
    }

    [HttpGet(AppConstants.AppController.IdentityController.AdminUser.GetAllUser)]
    // [Authorize(Roles = "Admin")]
    [Authorize(Policy = AppPolicy.ReadUser)]
    public async Task<IActionResult> GetUsers()
    {
        var users = (await _userService.GetUsersAsync())
            .Select(u => u.ToGetUserDto());

        return Ok(users);
    }

    [HttpPut(AppConstants.AppController.IdentityController.UpdateProfile)]
    [Authorize(Policy = AppPolicy.UpdateProfile)] // Ensure RoleClaimType is defined in SecureP.Identity.Models
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        _logger.LogInformation($"Updating profile for user with email: {User.FindFirstValue(ClaimTypes.Email)}");
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized(new UpdateProfileResponse
            {
                StatusCode = StatusCodes.Status404NotFound,
                Success = "false",
                Message = AppResponses.UpdateProfileResponses.UserNotFound,
            });
        }

        var (success, errors) = await _userService.UpdateProfileAsync(userId, request);

        if (success)
        {
            return Ok(new UpdateProfileResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Success = "true",
                Message = AppResponses.UpdateProfileResponses.ProfileUpdated,
            });
        }

        return BadRequest(new UpdateProfileResponse
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Success = "false",
            Message = AppResponses.UpdateProfileResponses.ProfileNotUpdated,
            Errors = errors.ToDictionary(e => e.Code, e => e.Description)
        });
    }

    [HttpPut(AppConstants.AppController.IdentityController.ChangePassword)]
    [Authorize(Policy = AppPolicy.ChangePassword)] // Ensure RoleClaimType is defined in SecureP.Identity.Models
    public async Task<IActionResult> ChangePassword([FromBody] UpdatePasswordRequest request)
    {
        _logger.LogInformation($"Changing password for user with email: {User.FindFirstValue(ClaimTypes.Email)}");
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized(new UpdatePasswordResponse
            {
                StatusCode = StatusCodes.Status404NotFound,
                Success = "false",
                Message = AppResponses.UpdatePasswordResponses.UserNotFound,
            });
        }

        if (request.OldPassword == request.NewPassword)
        {
            return BadRequest(new UpdatePasswordResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Success = "false",
                Message = AppResponses.UpdatePasswordResponses.NewPasswordIsSameAsOldPassword,
                Errors = AppResponseErrors.UpdatePasswordErrors.NewPasswordIsSameAsOldPassword
            });
        }

        var (success, errors) = await _userService.UpdatePasswordAsync(userId, request);

        if (!success)
        {
            var returnErrors = new Dictionary<string, object>();

            if (errors.Any(e => e.Code == "PasswordMismatch"))
            {
                returnErrors.Add("CurrentPassword", errors.First(e => e.Code == "PasswordMismatch").Description);
                returnErrors.Add("summary", "Invalid password");
            }

            if (errors.Any(e => e.Code != "PasswordMismatch"))
            {
                returnErrors.Add("NewPassword", errors.Where(e => e.Code != "PasswordMismatch").ToDictionary(e => e.Code, e => e.Description));
            }

            return BadRequest(new UpdatePasswordResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Success = "false",
                Message = AppResponses.UpdatePasswordResponses.PasswordNotUpdated,
                Errors = returnErrors
            });
        }

        return Ok(new UpdatePasswordResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Success = "true",
            Message = AppResponses.UpdatePasswordResponses.PasswordUpdated,
        });
    }

    [AllowAnonymous]
    [HttpPost(AppConstants.AppController.IdentityController.ForgotPassword)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        _logger.LogInformation($"Sending forgot password email to user with email: {request.Email}");

        await _userService.SendForgotPasswordEmailAsync(request.Email, request.RedirectUrl);

        return Ok(new ForgotPasswordResponse
        {
            StatusCode = StatusCodes.Status200OK,
            Success = "true",
            Message = AppResponses.ForgotPasswordResponses.ForgotPasswordEmailSent,
        });
    }

    [AllowAnonymous]
    [HttpPost(AppConstants.AppController.IdentityController.ResetPassword)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        _logger.LogInformation($"Resetting password for user with email: {request.Email}");

        var result = await _userService.ResetPasswordAsync(request.Email, request.Password, request.ConfirmPassword, request.Token);

        if (result.Succeeded)
        {
            return Ok(new ResetPasswordResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Success = "true",
                Message = AppResponses.ResetPasswordResponses.PasswordReset,
            });
        }

        var errors = result.Errors.ToDictionary(e => e.Code, e => e.Description);

        if (result.Errors == AppIdentityErrors.UnknownError)
        {
            errors.Add("summary", AppIdentityErrors.UnknownError.Description);

            return StatusCode(StatusCodes.Status500InternalServerError, new ResetPasswordResponse
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Success = "false",
                Message = AppResponses.ResetPasswordResponses.UnexpectedError,
                Errors = errors
            });
        }

        return BadRequest(new ResetPasswordResponse
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Success = "false",
            Message = AppResponses.ResetPasswordResponses.PasswordNotReset,
            Errors = errors
        });
    }
}