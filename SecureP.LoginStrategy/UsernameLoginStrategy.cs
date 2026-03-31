using SecureP.Identity.Models;
using SecureP.LoginStrategy.Abstraction;
using SecureP.Service.Abstraction;
using SecureP.Service.Abstraction.Entities;
using SecureP.Service.Abstraction.Results;
using SecureP.Shared;

namespace SecureP.LoginStrategy;

public class UsernameLoginStrategy<TKey>(IUserService<TKey> userService) : ILoginStrategy<TKey> where TKey : IEquatable<TKey>
{
    private readonly IUserService<TKey> _userService = userService;

    public LoginType AppliesTo => LoginType.Username;

    public async Task<Result<AppUser<TKey>?>> LoginAsync(LoginRequestDto request)
    {
        return await _userService.LoginByUsernameAsync(new LoginByUsernameRequest
        {
            Username = request.Username ?? string.Empty,
            Password = request.Password
        }, includeUserTokens: true, includeUserLogins: true);
    }
}
