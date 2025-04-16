using SecureP.Identity.Models;
using SecureP.Identity.Models.Dto;
using SecureP.Identity.Models.Dto.SortModels;
using SecureP.Identity.Models.Result;

namespace SecureP.Repository.Abstraction;

public interface IParkingRateRepository<TKey>
    where TKey : IEquatable<TKey>
{
    Task<ParkingRate<TKey>?> GetByIdAsync(TKey id);
    Task<List<ParkingRate<TKey>>> GetAllAsync(int page = 0, int limit = -1, ParkingRateOrderBy sort = ParkingRateOrderBy.HourlyRate, bool desc = false, string? search = null);
    Task AddAsync(ParkingRate<TKey> parkingRate);
    Task UpdateAsync(ParkingRate<TKey> parkingRate);
    Task DeleteAsync(ParkingRate<TKey> parkingRate);
    Task<int> CountAsync(int page = 0, int limit = -1, ParkingRateOrderBy sort = ParkingRateOrderBy.HourlyRate, bool desc = false, string? search = null);
    Task<int> SaveChangesAsync();
}