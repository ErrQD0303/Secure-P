using SecureP.Identity.Models;
using SecureP.Identity.Models.Dto;
using SecureP.Identity.Models.Dto.SortModels;
using SecureP.Identity.Models.Result;
using SecureP.Service.Abstraction.Entities;

namespace SecureP.Service.Abstraction;

public interface IParkingLocationService<TKey>
    where TKey : IEquatable<TKey>
{
    Task<GetParkingLocationDto<TKey>?> GetParkingLocationByIdAsync(TKey id);
    Task<GetAllParkingLocationsDto<TKey>?> GetParkingLocationsAsync(int pageIndex, int pageSize, ParkingLocationOrderBy orderBy, bool desc);
    Task<(ValidationResult, ParkingLocation<TKey>?)> CreateParkingLocationAsync(CreateParkingLocationRequest request);
    Task<ValidationResult> UpdateParkingLocationAsync(TKey id, UpdateParkingLocationRequest request);
    Task<bool> DeleteParkingLocationAsync(TKey id);
}