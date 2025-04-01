using SecureP.Identity.Models.Dto;
using SecureP.Repository.Abstraction;

namespace SecureP.Repository.ParkingLocations;

public class ParkingLocationRepository<TKey> : IParkingLocationRepository<TKey> where TKey : IEquatable<TKey>
{
    private readonly

    public Task<bool> CreateParkingLocationAsync(CreateParkingLocationDto<TKey> newParkingLocation)
    {
        throw new NotImplementedException();
    }

    public Task DeleteParkingLocationAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<GetParkingLocationDto<TKey>> GetParkingLocationByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<GetParkingLocationDto<TKey>>> GetParkingLocationsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateParkingLocationAsync(string id, UpdateParkingLocationDto<TKey> parkingLocation)
    {
        throw new NotImplementedException();
    }
}
