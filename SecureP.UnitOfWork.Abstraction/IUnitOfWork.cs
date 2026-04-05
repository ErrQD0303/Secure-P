using SecureP.Repository.Abstraction;

namespace SecureP.UnitOfWork.Abstraction;

public interface IUnitOfWork<TKey> : IDisposable
 where TKey : IEquatable<TKey>
{
    // Repositories
    IUserRepository<TKey> Users { get; }
    ITokenRepository<TKey> Tokens { get; }

    // Transaction Control
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();

    // Transaction Save Changes
    Task<int> CompleteAsync();
}