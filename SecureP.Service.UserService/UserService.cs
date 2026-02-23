using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SecureP.Identity.Models;
using SecureP.Identity.Models.Dto;
using SecureP.Identity.Models.Enum;
using SecureP.Service.Abstraction;
using SecureP.Service.Abstraction.Entities;
using SecureP.Service.Abstraction.Exceptions;
using SecureP.Shared;
using SecureP.Shared.Configures;
using SecureP.Shared.Helpers;
using SecureP.Shared.Mappers;

namespace SecureP.Service.UserService;

public class UserService<TKey> : IUserService<TKey> where TKey : IEquatable<TKey>
{
    private readonly ILogger<UserService<TKey>> _logger;
    private readonly UserManager<AppUser<TKey>> _userManager;
    private readonly IPasswordValidator<AppUser<TKey>> _passwordValidator;
    private readonly JwtConfigures _jwtConfigures;
    private readonly IEmailService _emailService;
    private readonly RoleManager<AppRole<TKey>> _roleManager;

    public UserService(ILogger<UserService<TKey>> logger, UserManager<AppUser<TKey>> userManager, IPasswordValidator<AppUser<TKey>> passwordValidator, IOptions<JwtConfigures> jwtConfiguresOptions, IEmailService emailService, RoleManager<AppRole<TKey>> roleManager)
    {
        _logger = logger;
        _userManager = userManager;
        _passwordValidator = passwordValidator;
        _jwtConfigures = jwtConfiguresOptions.Value;
        _emailService = emailService;
        _roleManager = roleManager;
    }

    public async Task<AppUser<TKey>?> GetUserByIdAsync(TKey id)
    {
        _logger.LogInformation($"Getting user with id: {id}");

        return await _userManager.Users
            .Include(u => u.UserTokens)
            .Include(u => u.UserLicensePlates)
            .FirstOrDefaultAsync(u => u.Id.Equals(id));
    }

    public async Task<IEnumerable<AppUser<TKey>>> GetUsersAsync()
    {
        _logger.LogInformation($"Getting all users");

        return await _userManager.Users
            .Include(u => u.UserTokens)
            .Include(u => u.UserLicensePlates)
            .ToListAsync();
    }

    public async Task<(bool Success, AppUser<TKey>? User)> LoginByEmailAsync(LoginByEmailRequest request)
    {
        if (string.IsNullOrEmpty(request.Email)) return (false, null);

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null) return (false, null);

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        return (isPasswordValid, user);
    }

    public async Task<(bool Success, AppUser<TKey>? User)> LoginByUsernameAsync(LoginByUsernameRequest request)
    {
        if (string.IsNullOrEmpty(request.Username)) return (false, null);

        var user = await _userManager.FindByNameAsync(request.Username);

        if (user == null) return (false, null);

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        return (isPasswordValid, user);
    }


    public async Task<(bool Success, AppUser<TKey>? User)> LoginByPhoneNumberAsync(LoginByPhoneNumberRequest request)
    {
        if (string.IsNullOrEmpty(request.Phone)) return (false, null);

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.Phone);

        if (user == null) return (false, null);

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

        return (isPasswordValid, user);
    }

    public async Task<AppUser<TKey>> RegisterAsync(RegisterRequest registerRequest)
    {
        _logger.LogInformation("Registering user with email: {email}", registerRequest.Email);

        var user = registerRequest.ToRegisterValidatedUserDto<TKey>();

        _logger.LogInformation("Validating user registration information for email: {email}", registerRequest.Email);
        await UserService<TKey>.ValidateUser(user, _userManager, _passwordValidator);

        var newUser = user.ToAppUser(_userManager);

        _logger.LogInformation("Creating user for email: {email}", registerRequest.Email);
        var result = await _userManager.CreateAsync(newUser, user.Password!);

        if (!result.Succeeded)
        {
            _logger.LogWarning("User registration failed for email: {email}. Errors: {errors}", registerRequest.Email, string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Description}")));
            throw new UserRegisterException("User registration failed", result.Errors);
        }

        _logger.LogInformation("User created successfully with email: {email}. Sending confirmation email.", registerRequest.Email);
        _ = SendConfirmationEmailAsync(newUser);
        _logger.LogInformation("User registered successfully with email: {email}", registerRequest.Email);
        return newUser;
    }

    /// <summary>
    /// Validates the user registration information
    /// </summary>
    /// <param name="user">the current user</param>
    /// <param name="userManager">User manager class</param>
    /// <param name="passwordValidator">the password validator object</param>
    /// <returns></returns>
    /// <exception cref="UserRegisterException">Represent the User Register Failures</exception>
    private static async Task ValidateUser(RegisterValidatedUserDto<TKey> user, UserManager<AppUser<TKey>> userManager, IPasswordValidator<AppUser<TKey>> passwordValidator)
    {
        // 1. Create empty error list
        var errors = new List<IdentityError>();

        // 2. Create a holder for validation results
        List<ValidationResult> validationResults = [];
        // 3. Create the registration user ValidationContext
        ValidationContext validationContext = new(user);
        // 4. Validate the registration user object and output the validation errors to the validationResults list
        if (!Validator.TryValidateObject(user, validationContext, validationResults, true))
        {
            // Add the validation errors to the error list, mapping the ValidationResult to IdentityError
            errors.AddRange(validationResults.Select(error => new IdentityError
            {
                Code = error.MemberNames.FirstOrDefault() ?? string.Empty,
                Description = error.ErrorMessage ?? string.Empty
            }));
        }

        // 5. Validate the password 
        var tempUser = new AppUser<TKey> { UserName = user.UserName ?? user.Email, Email = user.Email! };
        var passwordValidationResult = await passwordValidator.ValidateAsync(userManager, tempUser, user.Password);

        if (!passwordValidationResult.Succeeded)
        {
            errors.AddRange(passwordValidationResult.Errors);
        }


        // 6. Throw a UserRegisterException if there are any errors in the error list, passing the error list to the exception
        if (errors.Count != 0)
        {
            throw new UserRegisterException("User validation failed", errors);
        }
    }

    public Task<AppUser<TKey>?> GetUserByEmailAsync(string email)
    {
        return _userManager.FindByEmailAsync(email);
    }

    public Task<AppUser<TKey>?> GetUserByNameAsync(string username)
    {
        return _userManager.FindByNameAsync(username);
    }

    public async Task<bool> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Token))
            return false;

        var user = await GetUserByEmailAsync(request.Email);
        if (user == null) return false;

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));

        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

        return result.Succeeded;
    }

    /// <summary>
    /// Sends a confirmation email to the user
    /// </summary> 
    /// <param name="user">The user to send the confirmation email to</param>  
    /// <returns>A task that represents the asynchronous operation of sending the confirmation email</returns>
    private async Task SendConfirmationEmailAsync(AppUser<TKey> user)
    {
        if (user.Email == null)
        {
            _logger.LogWarning("Cannot send confirmation email to user with id: {userId} because email is 'null'", user.Id);
            return;
        }

        _logger.LogInformation("Generating email confirmation token for user with email: {email}", user.Email);
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var based64Token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        _logger.LogInformation("Constructing email confirmation URL for user with email: {email}", user.Email);
        var confirmEmailActionUrl = Path.Combine(_jwtConfigures.Authority!, AppConstants.DefaultRoutePrefix, AppConstants.AppController.IdentityController.DefaultRoute, AppConstants.AppController.IdentityController.ConfirmEmail)
            .Replace("\\", "/");
        var url = string.Format("{0}?email={1}&token={2}", confirmEmailActionUrl, UrlEncoder.Default.Encode(user.Email!), based64Token);

        // Send email
        _logger.LogInformation("Sending confirmation email to user with email: {email}", user.Email);
        await _emailService.SendConfirmationEmailAsync(user.Email, url);
    }

    public async Task ResendConfirmationEmailAsync(string email)
    {
        var user = await GetUserByEmailAsync(email);
        if (user == null) return;

        _ = SendConfirmationEmailAsync(user);
    }

    public async Task<bool> UpdateUserAsync(AppUser<TKey> user)
    {
        if (user is null) return false;

        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded;
    }

    private static async Task<(bool, IEnumerable<IdentityError>, AppUser<TKey>?)> IsUserValid(string userId, UserManager<AppUser<TKey>> userManager)
    {
        if (string.IsNullOrEmpty(userId)) return (false, new List<IdentityError> {
            new() {
                Code = "UserIdCannotBeEmpty",
                Description = "UserId cannot be null or empty"
            }}
        , null);

        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return (false, new List<IdentityError> {
            new() {
                Code = "UserNotFound",
                Description = "User not found"
            }
        }, null);

        return (true, Enumerable.Empty<IdentityError>(), user);
    }

    public async Task<(bool, IEnumerable<IdentityError>)> UpdateProfileAsync(string userId, UpdateProfileRequest request)
    {
        var (isUserValid, errors, user) = await IsUserValid(userId, _userManager);

        if (!isUserValid) return (false, errors);

        var oldEmail = user!.Email;

        _ = AppModelValidator.ValidateUserProfile(request.Email, request.PhoneNumber, request.DayOfBirth, out var selfValidateErrors);

        if (selfValidateErrors.Count != 0)
        {
            return (false, selfValidateErrors);
        }

        if (user!.Email != request.Email)
        {
            var emailExists = await _userManager.Users.AnyAsync(u => u.Email == request.Email);

            if (emailExists)
            {
                return (false, new List<IdentityError> {
                    new() {
                        Code = "Email",
                        Description = "Email already exists"
                    }
                });
            }

            user.Email = request.Email;
            user.NormalizedEmail = request.Email.ToUpper();
            user.EmailConfirmed = false;
        }

        if (user.PhoneNumber != request.PhoneNumber)
        {
            user.PhoneNumber = request.PhoneNumber;
        }

        if (user.DayOfBirth != request.DayOfBirth)
        {
            user.DayOfBirth = request.DayOfBirth;
        }

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded && oldEmail != request.Email)
        {
            _ = SendConfirmationEmailAsync(user);
        }

        return (result.Succeeded, result.Errors);
    }

    public async Task<(bool, IEnumerable<IdentityError>)> UpdatePasswordAsync(string userId, UpdatePasswordRequest request)
    {
        var (isUserValid, errors, user) = await IsUserValid(userId, _userManager);
        if (!isUserValid) return (false, errors);

        var result = await _userManager.ChangePasswordAsync(user!, request.OldPassword, request.NewPassword);

        return (result.Succeeded, result.Errors);
    }

    public async Task<IdentityResult> SendForgotPasswordEmailAsync(string email, string redirectUrl)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null) return IdentityResult.Failed(AppIdentityErrors.UserNotFound);

        if (user.EmailConfirmed == false)
        {
            return IdentityResult.Failed(AppIdentityErrors.EmailNotConfirmed);
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var based64Token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var forgotPasswordActionUrl = Path.Combine(redirectUrl)
            .Replace("\\", "/");

        var url = string.Format("{0}?email={1}&token={2}", forgotPasswordActionUrl, UrlEncoder.Default.Encode(user.Email!), based64Token);

        _ = _emailService.SendForgotPasswordEmailAsync(user.Email!, url);

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> ResetPasswordAsync(string email, string password, string confirmPassword, string token)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword) || string.IsNullOrEmpty(token))
        {
            return IdentityResult.Failed(AppIdentityErrors.InvalidRequest);
        }

        var user = await _userManager.FindByEmailAsync(email);

        if (user == null) return IdentityResult.Failed(AppIdentityErrors.InvalidEmail);

        if (user.EmailConfirmed == false)
        {
            return IdentityResult.Failed(AppIdentityErrors.EmailNotConfirmed);
        }

        if (password != confirmPassword)
        {
            return IdentityResult.Failed(AppIdentityErrors.InvalidConfirmPassword);
        }

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        var result = await _userManager.ResetPasswordAsync(user, decodedToken, password);

        if (result.Succeeded)
        {
            return IdentityResult.Success;
        }

        if (result.Errors.Any(e => e.Code == "InvalidToken"))
        {
            return IdentityResult.Failed(AppIdentityErrors.InvalidToken);
        }

        if (result.Errors.Any(e => e.Code == "InvalidPassword"))
        {
            return IdentityResult.Failed(AppIdentityErrors.InvalidPassword);
        }

        if (result.Errors.Any(e => e.Code == "InvalidEmail"))
        {
            return IdentityResult.Failed(AppIdentityErrors.InvalidEmail);
        }

        if (result.Errors.Any(e => e.Code == "InvalidRequest"))
        {
            return IdentityResult.Failed(AppIdentityErrors.InvalidRequest);
        }

        return IdentityResult.Failed(AppIdentityErrors.UnknownError);
    }

    public async Task<List<string>> GetUserPermissionsAsync(TKey id)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));
        if (user == null) return [];

        var userRolesName = await _userManager.GetRolesAsync(user);

        if (userRolesName is null || userRolesName.Count == 0)
        {
            return [];
        }

        var requestUserRoles = await _roleManager.Roles
            .Where(role => userRolesName.Contains(role.Name ?? string.Empty))
            .Where(role => role.UserRoles.Any(ur => ur.UserId.Equals(user.Id)))
            .Include(role => role.RoleClaims)
            .ToListAsync() ?? [];

        var permissions = new List<string>();
        foreach (var roleClaim in requestUserRoles.SelectMany(role => role.RoleClaims))
        {
            foreach (var claimType in Enum.GetValues<RoleClaimType>().Cast<RoleClaimType>())
            {
                if (roleClaim.ClaimValue == RoleClaimType.Administrator || (claimType & roleClaim.ClaimValue) == claimType)
                {
                    permissions.Add(claimType.ToString());
                }
            }
        }

        return [.. permissions.Distinct()];
    }

    public async Task<List<string>> GetUserRolesAsync(TKey id)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));
        if (user == null) return [];

        return [.. await _userManager.GetRolesAsync(user)];
    }
}
