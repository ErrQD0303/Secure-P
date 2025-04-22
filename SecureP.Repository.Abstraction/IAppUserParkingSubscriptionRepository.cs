using SecureP.Identity.Models;
using SecureP.Identity.Models.Dto.SortModels;

namespace SecureP.Repository.Abstraction;

public interface IAppUserParkingSubscriptionRepository<TKey> where TKey : IEquatable<TKey>
{
    Task<AppUserParkingSubscription<TKey>?> GetByIdAsync(TKey id);
    Task<List<AppUserParkingSubscription<TKey>>> GetAllAsync(int page = 0, int limit = -1, AppUserParkingSubscriptionOrderBy sort = AppUserParkingSubscriptionOrderBy.UserId, bool desc = false, string? search = null);
    Task AddAsync(AppUserParkingSubscription<TKey> appUserParkingSubscription);
    Task UpdateAsync(AppUserParkingSubscription<TKey> appUserParkingSubscription);
    Task DeleteAsync(AppUserParkingSubscription<TKey> appUserParkingSubscription);
    Task<int> CountAsync(int page = 0, int limit = -1, AppUserParkingSubscriptionOrderBy sort = AppUserParkingSubscriptionOrderBy.UserId, bool desc = false, string? search = null);
    Task<int> SaveChangesAsync();
}