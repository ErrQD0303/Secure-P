using SecureP.Identity.Models.Dto;
using SecureP.Identity.Models.Dto.SortModels;
using SecureP.Identity.Models.Result;
using SecureP.Service.Abstraction.Entities;

namespace SecureP.Service.Abstraction;

public interface IAppUserParkingSubscriptionService<TKey> where TKey : IEquatable<TKey>
{
    Task<GetAppUserParkingSubscriptionDto<TKey>?> GetAppUserParkingSubscriptionByIdAsync(TKey id);
    Task<GetAllAppUserParkingSubscriptionDto<TKey>?> GetAppUserParkingSubscriptionsAsync(int page = 1, int limit = -1, AppUserParkingSubscriptionOrderBy sort = AppUserParkingSubscriptionOrderBy.UserId, bool desc = false, string? search = null);
    Task<(ValidationResult, CreatedAppUserParkingSubscriptionDto<TKey>?)> CreateAppUserParkingSubscriptionAsync(CreateAppUserParkingSubscriptionDto<TKey> createDto);
    Task<ValidationResult> UpdateAppUserParkingSubscriptionAsync(UpdateAppUserParkingSubscriptionDto<TKey> updateDto);
    Task<bool> DeleteAppUserParkingSubscriptionAsync(TKey id);
}