using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SecureP.Identity.Models;
using SecureP.Identity.Models.Dto;
using SecureP.Service.Abstraction;
using SecureP.Service.Abstraction.Entities;
using SecureP.Service.Abstraction.Exceptions;
using SecureP.Shared.Mappers;

namespace SecureP.Service.UserService;

public class UserService<TKey> : IUserService<TKey> where TKey : IEquatable<TKey>
{
    private readonly ILogger<UserService<TKey>> _logger;
    private readonly UserManager<AppUser<TKey>> _userManager;
    private readonly IPasswordValidator<AppUser<TKey>> _passwordValidator;

    public UserService(ILogger<UserService<TKey>> logger, UserManager<AppUser<TKey>> userManager, IPasswordValidator<AppUser<TKey>> passwordValidator)
    {
        _logger = logger;
        _userManager = userManager;
        _passwordValidator = passwordValidator;
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
}
