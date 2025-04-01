using SecureP.Identity.Models;
using SecureP.Service.Abstraction.Entities;

namespace SecureP.Service.Abstraction;

public interface IParkingLocationService<TKey>
    where TKey : IEquatable<TKey>
{
    Task<ParkingLocation<TKey>> GetParkingLocationByIdAsync(TKey id);
    Task<IEnumerable<ParkingLocation<TKey>>> GetParkingLocationsAsync();
    Task<bool> CreateParkingLocationAsync(AddNewParkingLocationRequest request);
    Task<bool> UpdateParkingLocationAsync(TKey id, AddNewParkingLocationRequest request);
    Task DeleteParkingLocationAsync(TKey id);
}