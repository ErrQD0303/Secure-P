using SecureP.Shared;

namespace SecureP.LoginStrategy.Abstraction;

public interface ILoginStrategyFactory<TKey> where TKey : IEquatable<TKey>
{
    ILoginStrategy<TKey> GetStrategy(LoginType loginType);
}