using Microsoft.AspNetCore.Identity;
using SecureP.Identity.Models;
using SecureP.Service.Abstraction.Entities;

namespace SecureP.Service.Abstraction;

public interface IUserService<TKey> where TKey : IEquatable<TKey>
{
    Task<AppUser<TKey>?> GetUserByIdAsync(TKey id);
    Task<List<string>> GetUserPermissionsAsync(TKey id);
    Task<List<string>> GetUserRolesAsync(TKey id);
    Task<AppUser<TKey>?> GetUserByEmailAsync(string email);
    Task<AppUser<TKey>?> GetUserByNameAsync(string username);
    Task<IEnumerable<AppUser<TKey>>> GetUsersAsync();
    /// <summary>
    /// Register a new user with the provided registration details
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<AppUser<TKey>> RegisterAsync(RegisterRequest request);
    Task<(bool Success, AppUser<TKey>? User)> LoginByEmailAsync(LoginByEmailRequest request);
    Task<(bool Success, AppUser<TKey>? User)> LoginByUsernameAsync(LoginByUsernameRequest request);
    Task<bool> ConfirmEmailAsync(ConfirmEmailRequest request);
    Task ResendConfirmationEmailAsync(string email);
    Task<bool> UpdateUserAsync(AppUser<TKey> user);
    Task<(bool, IEnumerable<IdentityError>)> UpdateProfileAsync(string userId, UpdateProfileRequest request);
    Task<(bool, IEnumerable<IdentityError>)> UpdatePasswordAsync(string userId, UpdatePasswordRequest request);
    Task<IdentityResult> SendForgotPasswordEmailAsync(string email, string redirectUrl);
    Task<IdentityResult> ResetPasswordAsync(string email, string password, string confirmPassword, string token);
}