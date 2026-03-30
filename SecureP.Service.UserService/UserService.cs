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
using SecureP.Repository.Abstraction;
using SecureP.Service.Abstraction;
using SecureP.Service.Abstraction.Entities;
using SecureP.Service.Abstraction.Exceptions;
using SecureP.Service.Abstraction.Results;
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
    private readonly IEmailTaskQueue _emailTaskQueue;
    private readonly RoleManager<AppRole<TKey>> _roleManager;
    private readonly IUserRepository<TKey> _userRepository;

    public UserService(ILogger<UserService<TKey>> logger, UserManager<AppUser<TKey>> userManager, IPasswordValidator<AppUser<TKey>> passwordValidator, IOptions<JwtConfigures> jwtConfiguresOptions, IEmailTaskQueue emailTaskQueue, RoleManager<AppRole<TKey>> roleManager, IUserRepository<TKey> userRepository)
    {
        _logger = logger;
        _userManager = userManager;
        _passwordValidator = passwordValidator;
        _jwtConfigures = jwtConfiguresOptions.Value;
        _emailTaskQueue = emailTaskQueue;
        _roleManager = roleManager;
        _userRepository = userRepository;
    }

    public Task<AppUser<TKey>?> GetUserByIdAsync(TKey id)
    {
        return _userRepository.FindByIdAsync(id, includeUserTokens: true);
    }

    public async Task<IEnumerable<AppUser<TKey>>> GetUsersAsync()
    {
        _logger.LogInformation($"Getting all users");

        return await _userManager.Users
            .Include(u => u.UserTokens)
            .Include(u => u.UserLicensePlates)
            .ToListAsync();
    }

    public async Task<Result<AppUser<TKey>?>> LoginByEmailAsync(LoginByEmailRequest request)
    {
        var user = await _userRepository.FindByEmailAsync(request.Email, includeUserTokens: true, includeUserLogins: true);

        if (user == null) return Result<AppUser<TKey>?>.Failure([
            Error.Validation("Email", AppResponseErrors.UserLoginErrors.UserEmailNotFound["email"].ToString()!)
        ]);

        return await CheckPasswordThenReturnUserAsync(user, request.Password);
    }

    public async Task<Result<AppUser<TKey>?>> LoginByUsernameAsync(LoginByUsernameRequest request)
    {
        var user = await _userRepository.FindByUsernameAsync(request.Username, includeUserTokens: true, includeUserLogins: true);

        if (user == null) return Result<AppUser<TKey>?>.Failure([
            Error.Validation("Username", AppResponseErrors.UserLoginErrors.UserUsernameNotFound["username"].ToString()!)
        ]);

        return await CheckPasswordThenReturnUserAsync(user, request.Password);
    }

    private async Task<Result<AppUser<TKey>?>> CheckPasswordThenReturnUserAsync(AppUser<TKey> user, string password)
    {
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);

        return isPasswordValid ? Result<AppUser<TKey>?>.Success(user) : Result<AppUser<TKey>?>.Failure([
            Error.Validation("Password", AppResponseErrors.UserLoginErrors.UserPasswordInvalid["password"].ToString()!)
        ]);
    }

    public async Task<Result<AppUser<TKey>?>> LoginByPhoneNumberAsync(LoginByPhoneNumberRequest request)
    {
        var user = await _userRepository.FindByPhoneAsync(request.Phone, includeUserTokens: true, includeUserLogins: true);

        if (user == null)
        {
            return Result<AppUser<TKey>?>.Failure([
            Error.Validation("Phone", AppResponseErrors.UserLoginErrors.UserPhoneNumberNotFound["phone"].ToString()!)
        ]);
        }

        return await CheckPasswordThenReturnUserAsync(user, request.Password);
    }

    public async Task<Result<AppUser<TKey>>> RegisterAsync(RegisterRequest registerRequest)
    {
        _logger.LogInformation("Starting registration project for user with email: {email}", registerRequest.Email);

        var newUser = registerRequest.ToAppUser(_userManager);

        var result = await _userManager.CreateAsync(newUser, registerRequest.Password!);

        if (!result.Succeeded)
        {
            _logger.LogWarning("User registration failed for email: {email}. Errors: {errors}", registerRequest.Email, string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Description}")));

            return Result<AppUser<TKey>>.Failure([.. result.Errors.Select(e => Error.Validation(e.Code, e.Description))]);
        }

        _logger.LogInformation("User created successfully with email: {email}. Sending confirmation email.", registerRequest.Email);
        await SendConfirmationEmailAsync(newUser);
        return Result<AppUser<TKey>>.Success(newUser);
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
        var url = string.Format("{0}?email={1}&token={2}", confirmEmailActionUrl, Uri.EscapeDataString(user.Email), Uri.EscapeDataString(based64Token));

        // Send email
        _logger.LogInformation("Sending confirmation email to user with email: {email}", user.Email);
        await _emailTaskQueue.EnqueueEmailAsync(new SendEmailCommand(user.Email, url, AppConstants.SupportEmailType.ConfirmEmail));
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

        await _emailTaskQueue.EnqueueEmailAsync(new SendEmailCommand(user.Email!, url, AppConstants.SupportEmailType.ForgotPassword));

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
