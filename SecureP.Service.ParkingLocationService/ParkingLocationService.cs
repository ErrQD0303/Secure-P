using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SecureP.Identity.Models;
using SecureP.Service.Abstraction;
using SecureP.Service.Abstraction.Entities;

namespace SecureP.Service.ParkingLocationService;

public class ParkingLocationService<TKey> : IParkingLocationService<TKey>
    where TKey : IEquatable<TKey>
{
    private readonly ILogger<ParkingLocationService<TKey>> _logger;

    public ParkingLocationService(ILogger<ParkingLocationService<TKey>> logger)
    {
        _logger = logger;
    }

    public Task<bool> CreateParkingLocationAsync(AddNewParkingLocationRequest request)
    {

    }

    public Task DeleteParkingLocationAsync(TKey id)
    {
        throw new NotImplementedException();
    }

    public Task<ParkingLocation<TKey>> GetParkingLocationByIdAsync(TKey id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ParkingLocation<TKey>>> GetParkingLocationsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateParkingLocationAsync(TKey id, AddNewParkingLocationRequest request)
    {
        throw new NotImplementedException();
    }
}
