using Microsoft.AspNetCore.Identity;
using SecureP.Identity.Models;
using SecureP.Service.Abstraction.Entities;
using SecureP.Service.Abstraction.Results;

namespace SecureP.Service.Abstraction;

public interface IUserService<TKey> where TKey : IEquatable<TKey>
{
    Task<List<string>> GetUserPermissionsAsync(TKey id);
    Task<List<string>> GetUserRolesAsync(TKey id);
    Task<AppUser<TKey>?> GetUserByIdAsync(TKey id);
    Task<AppUser<TKey>?> GetUserByEmailAsync(string email);
    Task<AppUser<TKey>?> GetUserByUsernameAsync(string username);
    Task<IEnumerable<AppUser<TKey>>> GetUsersAsync();
    /// <summary>
    /// Register a new user with the provided registration details
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<Result<AppUser<TKey>>> RegisterAsync(RegisterRequest request);
    Task<Result<AppUser<TKey>?>> LoginByEmailAsync(LoginByEmailRequest request, bool includeUserTokens = false, bool includeUserRoles = false, bool includeUserLogins = false);
    Task<Result<AppUser<TKey>?>> LoginByUsernameAsync(LoginByUsernameRequest request, bool includeUserTokens = false, bool includeUserRoles = false, bool includeUserLogins = false);
    Task<Result<AppUser<TKey>?>> LoginByPhoneNumberAsync(LoginByPhoneNumberRequest request, bool includeUserTokens = false, bool includeUserRoles = false, bool includeUserLogins = false);
    Task<bool> ConfirmEmailAsync(ConfirmEmailRequest request);
    Task ResendConfirmationEmailAsync(string email);
    Task<bool> UpdateUserAsync(AppUser<TKey> user);
    Task<(bool, IEnumerable<IdentityError>)> UpdateProfileAsync(string userId, UpdateProfileRequest request);
    Task<(bool, IEnumerable<IdentityError>)> UpdatePasswordAsync(string userId, UpdatePasswordRequest request);
    Task<IdentityResult> SendForgotPasswordEmailAsync(string email, string redirectUrl);
    Task<IdentityResult> ResetPasswordAsync(string email, string password, string confirmPassword, string token);
}