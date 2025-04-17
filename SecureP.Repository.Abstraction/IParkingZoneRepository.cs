using SecureP.Identity.Models;
using SecureP.Identity.Models.Dto.SortModels;

namespace SecureP.Repository.Abstraction;

public interface IParkingZoneRepository<TKey>
    where TKey : IEquatable<TKey>
{
    Task<ParkingZone<TKey>?> GetByIdAsync(TKey id);
    Task<List<ParkingZone<TKey>>> GetAllAsync(int page = 0, int limit = -1, ParkingZoneOrderBy sort = ParkingZoneOrderBy.Name, bool desc = false, string? search = null);
    Task AddAsync(ParkingZone<TKey> parkingZone);
    Task UpdateAsync(ParkingZone<TKey> parkingZone);
    Task DeleteAsync(ParkingZone<TKey> parkingZone);
    Task<int> CountAsync(int page = 0, int limit = -1, ParkingZoneOrderBy sort = ParkingZoneOrderBy.Name, bool desc = false, string? search = null);
    Task<int> SaveChangesAsync();
}