using SecureP.Identity.Models;
using SecureP.LoginStrategy.Abstraction;
using SecureP.Service.Abstraction;
using SecureP.Service.Abstraction.Entities;
using SecureP.Service.Abstraction.Results;
using SecureP.Shared;

namespace SecureP.LoginStrategy;

public class PhoneLoginStrategy<TKey>(IUserService<TKey> userService) : ILoginStrategy<TKey> where TKey : IEquatable<TKey>
{
    private readonly IUserService<TKey> _userService = userService;

    public LoginType AppliesTo => LoginType.Phone;

    public async Task<Result<AppUser<TKey>?>> LoginAsync(LoginRequestDto request)
    {
        return await _userService.LoginByPhoneNumberAsync(new LoginByPhoneNumberRequest
        {
            Phone = request.Phone ?? string.Empty,
            Password = request.Password
        }, includeUserTokens: true, includeUserLogins: true);
    }
}
