using SecureP.LoginStrategy.Abstraction;
using SecureP.Shared;

namespace SecureP.LoginStrategy;

public class LoginStrategyFactory<TKey>(IEnumerable<ILoginStrategy<TKey>> strategies) : ILoginStrategyFactory<TKey> where TKey : IEquatable<TKey>
{
    private readonly IEnumerable<ILoginStrategy<TKey>> _strategies = strategies;

    public ILoginStrategy<TKey> GetStrategy(LoginType loginType)
    {
        var strategy = _strategies.FirstOrDefault(s => s.AppliesTo == loginType) ?? throw new NotSupportedException($"Login type {loginType} is not supported.");

        return strategy;
    }
}