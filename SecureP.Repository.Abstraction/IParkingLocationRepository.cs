using SecureP.Identity.Models.Dto;

namespace SecureP.Repository.Abstraction;

public interface IParkingLocationRepository<TKey>
    where TKey : IEquatable<TKey>
{
    Task<GetParkingLocationDto<TKey>> GetParkingLocationByIdAsync(string id);
    Task<IEnumerable<GetParkingLocationDto<TKey>>> GetParkingLocationsAsync();
    Task<bool> CreateParkingLocationAsync(CreateParkingLocationDto<TKey> parkingLocation);
    Task<bool> UpdateParkingLocationAsync(string id, UpdateParkingLocationDto<TKey> parkingLocation);
    Task DeleteParkingLocationAsync(string id);
}