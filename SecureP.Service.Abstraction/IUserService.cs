using SecureP.Identity.Models;
using SecureP.Service.Abstraction.Entities;

namespace SecureP.Service.Abstraction;

public interface IUserService<TKey> where TKey : IEquatable<TKey>
{
    Task<AppUser<TKey>?> GetUserByIdAsync(TKey id);
    Task<IEnumerable<AppUser<TKey>>> GetUsersAsync();
    Task<AppUser<TKey>?> RegisterAsync(RegisterRequest request);
    Task<(bool Success, AppUser<TKey>? User)> LoginByEmailAsync(LoginByEmailRequest request);
    Task<(bool Success, AppUser<TKey>? User)> LoginByUsernameAsync(LoginByUsernameRequest request);
}