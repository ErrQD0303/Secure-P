using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using SecureP.Data;
using SecureP.Identity.Models;
using SecureP.Repository.Abstraction;
using SecureP.Repository.Tokens;
using SecureP.Repository.UserRepository;
using SecureP.UnitOfWork.Abstraction;
using SecureP.UnitOfWork.Extensions;

namespace SecureP.UnitOfWork;

public class UnitOfWork<TKey> : IUnitOfWork<TKey>
 where TKey : IEquatable<TKey>
{
    // fields
    private bool _disposed = false;
    private readonly AppDbContext<TKey> _context;
    private IDbContextTransaction? _transaction = default!;

    // Repositories
    public IUserRepository<TKey> Users { get; init; }
    public ITokenRepository<TKey> Tokens { get; init; }

    // Constructor
    public UnitOfWork(AppDbContext<TKey> context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        Users = new UserRepository<TKey>(_context);
        Tokens = new TokenRepository<TKey>(_context);
    }

    // Transaction Control
    public Task BeginTransactionAsync()
    {
        if (_transaction is not null)
        {
            return Task.CompletedTask; // Transaction already started
        }

        _transaction = _context.Database.BeginTransaction();
        return Task.CompletedTask;
    }

    public async Task CommitAsync()
    {
        try
        {
            await CompleteAsync(); // Save changes before committing

            if (_transaction is not null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackAsync();
            throw; // Rethrow the exception after rollback. This will hold the stack trace and allow higher-level code to handle it appropriately.
        }
        finally
        {
            if (_transaction is not null)
            {
                await _transaction.DisposeTransactionAsync(); // This line ensures that the transaction is Disposed after the success or failure of the commit operation, preventing potential resource leaks.
            }
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction is null)
        {
            return;
        }

        await _transaction.RollbackAsync();
        await _transaction.DisposeTransactionAsync(); // This line ensures that the transaction is Disposed when you call RolbackAsync by yourself, preventing potential resource leaks.
    }


    // Transaction Save Changes
    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    // IDisposable Implementation
    public void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            // Dispose managed resources here

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
