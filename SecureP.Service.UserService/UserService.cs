using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SecureP.Identity.Models;
using SecureP.Service.Abstraction;
using SecureP.Service.Abstraction.Entities;

namespace SecureP.Service.UserService;

public class UserService<TKey> : IUserService<TKey> where TKey : IEquatable<TKey>
{
    private readonly ILogger<UserService<TKey>> _logger;
    private readonly UserManager<AppUser<TKey>> _userManager;

    public UserService(ILogger<UserService<TKey>> logger, UserManager<AppUser<TKey>> userManager)
    {
        _logger = logger;
        _userManager = userManager;
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

        var user = new AppUser<TKey>
        {
            UserName = registerRequest.Username ?? registerRequest.Email,
            Email = registerRequest.Email,
            EmailConfirmed = false,
            SecurityStamp = Guid.NewGuid().ToString(),
            TwoFactorEnabled = false,
            LockoutEnabled = _userManager.Options.Lockout.AllowedForNewUsers,
            PhoneNumber = registerRequest.PhoneNumber,
            FullName = registerRequest.FullName,
            DayOfBirth = registerRequest.DayOfBirth,
            Country = registerRequest.Country,
            City = registerRequest.City,
            AddressLine1 = registerRequest.AddressLine1,
            AddressLine2 = registerRequest.AddressLine2,
            PostCode = registerRequest.PostCode,
            Id = typeof(TKey) switch
            {
                Type t when t == typeof(string) => (TKey)(object)Guid.NewGuid().ToString(),
                Type t when t == typeof(Guid) => (TKey)(object)Guid.NewGuid().ToString(),
                _ => throw new NotImplementedException()
            }
        };

        user.UserLicensePlates = [.. registerRequest.LicensePlates.Select(lp => new AppUserLicensePlate<TKey>
            {
                LicensePlateNumber = lp,
                UserId = user.Id,
            })];

        var result = await _userManager.CreateAsync(user, registerRequest.Password);

        if (!result.Succeeded)
        {
            return null;
        }

        return user;
    }
}
