using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SecureP.Identity.Models;
using SecureP.Identity.Models.Dto;
using SecureP.Service.Abstraction;
using SecureP.Service.Abstraction.Entities;
using SecureP.Service.Abstraction.Exceptions;
using SecureP.Shared;
using SecureP.Shared.Configures;
using SecureP.Shared.Mappers;

namespace SecureP.Service.UserService;

public class UserService<TKey> : IUserService<TKey> where TKey : IEquatable<TKey>
{
    private readonly ILogger<UserService<TKey>> _logger;
    private readonly UserManager<AppUser<TKey>> _userManager;
    private readonly IPasswordValidator<AppUser<TKey>> _passwordValidator;
    private readonly JwtConfigures _jwtConfigures;
    private readonly IEmailService _emailService;

    public UserService(ILogger<UserService<TKey>> logger, UserManager<AppUser<TKey>> userManager, IPasswordValidator<AppUser<TKey>> passwordValidator, IOptions<JwtConfigures> jwtConfiguresOptions, IEmailService emailService)
    {
        _logger = logger;
        _userManager = userManager;
        _passwordValidator = passwordValidator;
        _jwtConfigures = jwtConfiguresOptions.Value;
        _emailService = emailService;
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

    public async Task<AppUser<TKey>?> RegisterAsync(RegisterRequest registerRequest)
    {
        _logger.LogInformation($"Registering user with email: {registerRequest.Email}");

        var user = registerRequest.ToRegisterValidatedUserDto<TKey>();

        await UserService<TKey>.ValidateUser(user, _userManager, _passwordValidator);

        var newUser = user.ToAppUser(_userManager);

        var result = await _userManager.CreateAsync(newUser, user.Password!);

        if (!result.Succeeded)
        {
            return null;
        }

        _ = SendConfirmationEmailAsync(newUser);
        return newUser;
    }

    private static async Task ValidateUser(RegisterValidatedUserDto<TKey> user, UserManager<AppUser<TKey>> userManager, IPasswordValidator<AppUser<TKey>> passwordValidator)
    {
        var errors = new List<IdentityError>();

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(user);
        if (!Validator.TryValidateObject(user, validationContext, validationResults, true))
        {
            foreach (var error in validationResults)
            {
                errors.Add(new IdentityError
                {
                    Code = error.MemberNames.FirstOrDefault() ?? string.Empty,
                    Description = error.ErrorMessage ?? string.Empty
                });
            }
        }

        if (string.IsNullOrEmpty(user.Password))
        {
            errors.Add(new IdentityError
            {
                Code = "PasswordCannotBeEmpty",
                Description = "Password cannot be null or empty"
            });
        }
        else
        {
            var tempUser = new AppUser<TKey> { UserName = user.UserName ?? user.Email, Email = user.Email! };
            var passwordValidationResult = await passwordValidator.ValidateAsync(userManager, tempUser, user.Password);

            if (!passwordValidationResult.Succeeded)
            {
                errors.AddRange(passwordValidationResult.Errors);
            }
        }

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

    private async Task SendConfirmationEmailAsync(AppUser<TKey> user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var based64Token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var confirmEmailActionUrl = Path.Combine(_jwtConfigures.Authority!, AppConstants.DefaultRoutePrefix, AppConstants.AppController.IdentityController.DefaultRoute, AppConstants.AppController.IdentityController.ConfirmEmail)
            .Replace("\\", "/");
        var url = string.Format("{0}?email={1}&token={2}", confirmEmailActionUrl, UrlEncoder.Default.Encode(user.Email!), based64Token);

        if (user.Email == null) return;

        // Send email
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
}
