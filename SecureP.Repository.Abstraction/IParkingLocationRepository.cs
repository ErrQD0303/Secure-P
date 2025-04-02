using SecureP.Identity.Models;
using SecureP.Identity.Models.Dto;
using SecureP.Identity.Models.Dto.SortModels;
using SecureP.Identity.Models.Result;

namespace SecureP.Repository.Abstraction;

public interface IParkingLocationRepository<TKey>
    where TKey : IEquatable<TKey>
{
    Task<GetParkingLocationDto<TKey>?> GetParkingLocationByIdAsync(TKey id);
    Task<GetAllParkingLocationsDto<TKey>?> GetParkingLocationsAsync(int pageIndex, int pageSize, ParkingLocationOrderBy orderBy, bool desc);
    Task<(ValidationResult, ParkingLocation<TKey>?)> CreateParkingLocationAsync(CreateParkingLocationDto parkingLocation);
    Task<ValidationResult> UpdateParkingLocationAsync(TKey id, UpdateParkingLocationDto parkingLocation);
    Task<bool> DeleteParkingLocationAsync(TKey id);
}