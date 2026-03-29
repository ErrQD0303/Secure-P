using System.ComponentModel.DataAnnotations;
using SecureP.Service.Abstraction.Results; // Ensure this namespace is imported

namespace Secure_P_Backend.Controllers;

/// <summary>
/// Controller that handles user identity-related operations such as registration, login, logout, email confirmation, password reset, and profile updates for the Secure-P Backend application. The IdentityController provides endpoints for managing user accounts and authentication processes, allowing users to securely access the application's features based on their subscription status and permissions. Each action method in this controller is designed to handle specific identity-related tasks while ensuring proper authorization and error handling mechanisms are in place to maintain the security and integrity of user data.
/// </summary>
[ApiController]
[Route(AppConstants.AppController.IdentityController.DefaultRoute)]
public class IdentityController : ControllerBase
{
    private readonly ILogger<IdentityController> _logger;
    private readonly IUserService<string> _userService;
    private readonly ITokenService<string> _tokenService;
    private readonly JwtConfigures _jwtConfigures;
    private readonly IEmailTaskQueue _emailTaskQueue;
    private readonly ILoginStrategyFactory<string> _loginStrategyFactory;

    public IdentityController(ILogger<IdentityController> logger, IUserService<string> userService, ITokenService<string> tokenService, IOptions<JwtConfigures> jwtConfigures, IEmailTaskQueue emailTaskQueue, ILoginStrategyFactory<string> loginStrategyFactory)
    {
        _logger = logger;
        _userService = userService;
        _tokenService = tokenService;
        _jwtConfigures = jwtConfigures.Value;
        _emailTaskQueue = emailTaskQueue;
        _loginStrategyFactory = loginStrategyFactory;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="registerRequest"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost(AppConstants.AppController.IdentityController.Register)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        _logger.LogInformation("Registering user with email: {email}", registerRequest.Email);

        // Register user
        var userResult = await _userService.RegisterAsync(registerRequest);
        if (!userResult.IsSuccess)
        {
            var errors = userResult.Errors.ToDictionary(e => e.Code, e => (object)e.Description);

            return CreateUserRegisterFailedResponse(errors);
        }

        var userDto = new UserRegisterResponse<string>
        {
            StatusCode = StatusCodes.Status201Created,
            Success = "true",
            Message = AppResponses.UserRegisterResponses.UserRegistered,
            User = userResult.Value!.ToUserRegisterResponseAppUser()
        };

        return CreatedAtAction(nameof(GetUserInfo), new { id = userResult.Value!.Id }, userDto);
    }

    [NonAction]
    private BadRequestObjectResult CreateUserRegisterFailedResponse(Dictionary<string, object> additionalErrors)
    {
        var errors = AppResponseErrors.UserRegisterErrors.UserRegistrationFailed
            .Concat(additionalErrors)
            .ToDictionary(x => x.Key, x => x.Value);

        return BadRequest(new UserRegisterResponse<string>
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Success = "false",
            Message = AppResponses.UserRegisterResponses.UserRegistrationFailed,
            Errors = errors
        });
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
        await tokenService.InvalidateAccessAndRefreshTokensAsync(userId);

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
    public async Task<IActionResult> Login([FromRoute(Name = "login-type")] LoginType loginType, [FromBody] LoginRequestDto loginRequest)
    {
        // Login user
        var loginResult = await _loginStrategyFactory
                                    .GetStrategy(loginType)
                                    .LoginAsync(loginRequest);

        if (!loginResult.IsSuccess)
        {
            _logger.LogWarning("Login failed for user: {emailOrUsername}", loginRequest.Email ?? loginRequest.Username ?? string.Empty);

            return Unauthorized(new LoginResponse<string>
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Success = "false",
                Message = AppResponses.UserLoginResponses.UserLoginFailed,
                Errors = AppResponseErrors.UserLoginErrors.UserLoginFailed
            });
        }

        if (string.IsNullOrWhiteSpace(loginResult.Value?.Email))
        {
            // Response.Cookies.Append(AppConstants.OTPConstant.TemporaryCookieName, user.Email, new CookieOptions
            // {
            //     HttpOnly = true, // Turn off JS accessibility
            //     Secure = true, // Allow cookie to be sent over HTTP // Set to true for production
            //     SameSite = SameSiteMode.None, // Adjusted for frontend compatibility // Set to Strict for production
            //     Expires = DateTime.UtcNow.AddMinutes(AppConstants.OTPConstant.ExpiryMinute),
            //     Path = "/",
            //     // Domain = "localhost" // Ensure this matches the domain used when setting the cookie
            // });

            _logger.LogError("User {emailOrUsername} does not have an email, cannot proceed with OTP generation", loginRequest.Email ?? loginRequest.Username ?? string.Empty);
            return BadRequest(new LoginResponse<string>
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Success = "false",
                Message = AppResponses.UserLoginResponses.UserEmailNotFound,
                Errors = AppResponseErrors.UserLoginErrors.UserEmailNotFound
            });
        }

        _logger.LogInformation("User {email} logged in successfully, generating OTP for 2FA", loginResult.Value?.Email);
        var user = loginResult.Value;
        var otp = await _tokenService.GenerateOTPAsync(user!);

        await _emailTaskQueue.EnqueueEmailAsync(new SendEmailCommand(loginResult.Value?.Email!, otp, AppConstants.SupportEmailType.OTP));

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
        _logger.LogInformation("Logging in user with email: {email} using OTP", request.Email);
        var result = await _tokenService.ValidateOTPAsync(request.Email, request.OTP);

        if (!result.IsSuccess)
        {
            var reason = result.Errors.FirstOrDefault()?.Description ?? "Unknown error";
            _logger.LogWarning("OTP login failed for user with email: {email}. Reason: {reason}", request.Email, reason);

            return Unauthorized(new LoginResponse<string>
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Success = "false",
                Message = reason,
                Errors = result.Errors.ToDictionary(e => e.Code, e => (object)e.Description)
            });
        }

        var user = result.Value!;

        // Response.Cookies.Append(AppConstants.OTPConstant.TemporaryCookieName, user.Email!, new CookieOptions
        // {
        //     HttpOnly = true, // Turn off JS accessibility
        //     Secure = true, // Allow cookie only for HTTPS // Set to true for production
        //     SameSite = SameSiteMode.None, // Adjusted for frontend compatibility // Set to Strict for production
        //     Expires = DateTime.UtcNow,
        //     Path = "/",
        //     Domain = "localhost" // Ensure this matches the domain used when setting the cookie
        // });

        var tokens = await SetAccessCookies(request, user, Response, _tokenService, _jwtConfigures);
        _logger.LogInformation("User with email: {email} logged in successfully with OTP", request.Email);

        return Ok(new LoginResponse<string>
        {
            StatusCode = StatusCodes.Status200OK,
            Success = "true",
            Message = AppResponses.UserLoginResponses.UserLoggedIn,
            User = user.ToLoginResponseAppUser(),
            Tokens = tokens
        });
    }

    [NonAction]
    public static async Task<TokenResponse> SetAccessCookies(dynamic? requestData, AppUser<string> user, HttpResponse Response, ITokenService<string> tokenService, JwtConfigures jwtConfigures)
    {
        var accessAndRefreshTokens = await tokenService.GenerateAccessAndRefreshTokensAsync(user);
        var tokenResponse = new TokenResponse
        {
            AccessToken = accessAndRefreshTokens.AccessToken,
            RefreshToken = accessAndRefreshTokens.RefreshToken,
            TokenType = AppConstants.JwtScheme
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

        ValidationContext context = new(request);
        List<ValidationResult> validationResults = [];
        bool isValid = Validator.TryValidateObject(request, context, validationResults, true);

        if (isValid && await _userService.ConfirmEmailAsync(request))
        {
            return Ok(new ConfirmEmailResponse<string>
            {
                StatusCode = StatusCodes.Status200OK,
                Success = "true",
                Message = AppResponses.EmailConfirmationResponses.EmailConfirmed,
            });
        }

        var errors = !isValid
            ? AppResponseErrors.EmailConfirmationErrors.EmailConfirmationFailed
                .Concat(validationResults.ToDictionary(vr => vr.MemberNames.FirstOrDefault()!, v => v.ErrorMessage!))
                .ToDictionary(x => x.Key, x => x.Value)
            : AppResponseErrors.EmailConfirmationErrors.EmailConfirmationFailed;

        return BadRequest(new ConfirmEmailResponse<string>
        {
            StatusCode = StatusCodes.Status400BadRequest,
            Success = "false",
            Message = AppResponses.EmailConfirmationResponses.EmailConfirmationFailed,
            Errors = errors
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