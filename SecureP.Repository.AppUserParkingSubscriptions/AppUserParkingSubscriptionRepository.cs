using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SecureP.Data;
using SecureP.Identity.Models;
using SecureP.Identity.Models.Dto.SortModels;
using SecureP.Repository.Abstraction;

namespace SecureP.Repository.AppUserParkingSubscriptions;

public class AppUserParkingSubscriptionRepository<TKey> : IAppUserParkingSubscriptionRepository<TKey> where TKey : IEquatable<TKey>
{
    public ILogger<AppUserParkingSubscriptionRepository<TKey>> Logger { get; private set; }
    public AppDbContext<TKey> Context { get; private set; }
    public AppUserParkingSubscriptionRepository(ILogger<AppUserParkingSubscriptionRepository<TKey>> logger, AppDbContext<TKey> context)
    {
        Logger = logger;
        Context = context;
    }

    private static IQueryable<AppUserParkingSubscription<TKey>?> GetQueryableAsync(IQueryable<AppUserParkingSubscription<TKey>> query, int page, int limit, AppUserParkingSubscriptionOrderBy sort, bool desc, string? search)
    {
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(x =>
                (x != null && x.User != null && x.User.UserName != null && x.User.UserName.Contains(search)) ||
                (x != null && x.User != null && x.User.FullName != null && x.User.FullName.Contains(search)) ||
                (x != null && x.User != null && x.User.Email != null && x.User.Email.Contains(search)) || (x != null && x.ParkingZone.Name != null && x.ParkingZone.Name.Contains(search)) || (x != null && x.ParkingZone != null && x.ParkingZone.ParkingLocation != null && x.ParkingZone.ParkingLocation.Name != null && x.ParkingZone.ParkingLocation.Name.Contains(search)));
        }

        query = sort switch
        {
            AppUserParkingSubscriptionOrderBy.Id => desc ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id),
            AppUserParkingSubscriptionOrderBy.UserId => desc ? query.OrderByDescending(x => x.UserId) : query.OrderBy(x => x.UserId),
            AppUserParkingSubscriptionOrderBy.ParkingZoneId => desc ? query.OrderByDescending(x => x.ParkingZoneId) : query.OrderBy(x => x.ParkingZoneId),
            AppUserParkingSubscriptionOrderBy.SubscriptionStartDate => desc ? query.OrderByDescending(x => x.StartDate) : query.OrderBy(x => x.StartDate),
            AppUserParkingSubscriptionOrderBy.SubscriptionEndDate => desc ? query.OrderByDescending(x => x.EndDate) : query.OrderBy(x => x.EndDate),
            AppUserParkingSubscriptionOrderBy.SubscriptionStatus => desc ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            AppUserParkingSubscriptionOrderBy.ProductType => desc ? query.OrderByDescending(x => x.ProductType) : query.OrderBy(x => x.ProductType),
            AppUserParkingSubscriptionOrderBy.LicensePlate => desc ? query.OrderByDescending(x => x.LicensePlate) : query.OrderBy(x => x.LicensePlate),
            AppUserParkingSubscriptionOrderBy.SubscriptionFee => desc ? query.OrderByDescending(x => x.SubscriptionFee) : query.OrderBy(x => x.SubscriptionFee),
            AppUserParkingSubscriptionOrderBy.ClampingFee => desc ? query.OrderByDescending(x => x.ClampingFee) : query.OrderBy(x => x.ClampingFee),
            AppUserParkingSubscriptionOrderBy.ChangeSignageFee => desc ? query.OrderByDescending(x => x.ChangeSignageFee) : query.OrderBy(x => x.ChangeSignageFee),
            AppUserParkingSubscriptionOrderBy.PaymentDate => desc ? query.OrderByDescending(x => x.PaymentDate) : query.OrderBy(x => x.PaymentDate),
            AppUserParkingSubscriptionOrderBy.IsPaid => desc ? query.OrderByDescending(x => x.IsPaid) : query.OrderBy(x => x.IsPaid),
            _ => throw new ArgumentOutOfRangeException(nameof(sort), sort, null)
        };

        query = query.Skip(page * (limit > 0 ? limit : 1));
        if (limit > 0)
        {
            query = query.Take(limit);
        }
        return query;
    }

    public async Task AddAsync(AppUserParkingSubscription<TKey> appUserParkingSubscription)
    {
        await Context.UserParkingSubscriptions.AddAsync(appUserParkingSubscription);
    }

    public Task<int> CountAsync(int page = 0, int limit = -1, AppUserParkingSubscriptionOrderBy sort = AppUserParkingSubscriptionOrderBy.UserId, bool desc = false, string? search = null)
    {
        return GetQueryableAsync(Context.UserParkingSubscriptions.AsQueryable(), page, limit, sort, desc, search).CountAsync();
    }

    public Task DeleteAsync(AppUserParkingSubscription<TKey> appUserParkingSubscription)
    {
        Context.UserParkingSubscriptions.Remove(appUserParkingSubscription);
        return Task.CompletedTask;
    }

    public Task<List<AppUserParkingSubscription<TKey>>> GetAllAsync(int page = 0, int limit = -1, AppUserParkingSubscriptionOrderBy sort = AppUserParkingSubscriptionOrderBy.UserId, bool desc = false, string? search = null)
    {
        var query = GetQueryableAsync(Context.UserParkingSubscriptions.AsQueryable(), page, limit, sort, desc, search);

        return query.Where(x => x != null).Cast<AppUserParkingSubscription<TKey>>().ToListAsync();
    }

    public async Task<AppUserParkingSubscription<TKey>?> GetByIdAsync(TKey id)
    {
        return await Context.UserParkingSubscriptions.FindAsync(id);
    }

    public Task<int> SaveChangesAsync()
    {
        return Context.SaveChangesAsync();
    }

    public Task UpdateAsync(AppUserParkingSubscription<TKey> appUserParkingSubscription)
    {
        Context.UserParkingSubscriptions.Update(appUserParkingSubscription);
        return Task.CompletedTask;
    }
}
