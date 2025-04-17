using SecureP.Identity.Models.Dto;
using SecureP.Identity.Models.Dto.SortModels;
using SecureP.Identity.Models.Result;
using SecureP.Service.Abstraction.Entities;

namespace SecureP.Service.Abstraction;

public interface IParkingZoneService<TKey> where TKey : IEquatable<TKey>
{
    Task<GetParkingZoneDto<TKey>?> GetParkingZoneByIdAsync(TKey id);
    Task<GetAllParkingZoneDto<TKey>?> GetParkingZonesAsync(int page = 1, int limit = -1, ParkingZoneOrderBy sort = ParkingZoneOrderBy.Name, bool desc = false, string? search = null);
    Task<(ValidationResult, CreatedParkingZoneDto<TKey>?)> CreateParkingZoneAsync(CreateParkingZoneDto<TKey> request);
    Task<ValidationResult> UpdateParkingZoneAsync(UpdateParkingZoneDto<TKey> request);
    Task<bool> DeleteParkingZoneAsync(TKey id);
}