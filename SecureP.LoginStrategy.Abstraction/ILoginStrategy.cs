using SecureP.Identity.Models;
using SecureP.Service.Abstraction.Entities;
using SecureP.Service.Abstraction.Results;
using SecureP.Shared;

namespace SecureP.LoginStrategy.Abstraction;

public interface ILoginStrategy<TKey> where TKey : IEquatable<TKey>
{
    /// <summary>
    /// The type of login this strategy applies to.
    /// </summary>
    LoginType AppliesTo { get; }

    /// <summary>
    /// The Login Logic for the specific LoginType.
    /// </summary>
    Task<Result<AppUser<TKey>?>> LoginAsync(LoginRequestDto request);
}
