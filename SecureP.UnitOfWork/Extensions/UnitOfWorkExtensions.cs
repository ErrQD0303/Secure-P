using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using SecureP.UnitOfWork.Abstraction;

namespace SecureP.UnitOfWork.Extensions;

public static class UnitOfWorkExtensions
{

    // Helpers
    public static async Task DisposeTransactionAsync(this IDbContextTransaction? transaction)
    {
        await transaction!.DisposeAsync();
        transaction = null;
    }

    public static IServiceCollection AddUnitOfWork<TKey>(this IServiceCollection services) where TKey : IEquatable<TKey>
    {
        services.AddScoped<IUnitOfWork<TKey>, UnitOfWork<TKey>>();
        return services;
    }
}