using Microsoft.Extensions.Logging;
using SecureP.Identity.Models;
using SecureP.Identity.Models.Dto;
using SecureP.Identity.Models.Dto.SortModels;
using SecureP.Identity.Models.Result;
using SecureP.Repository.Abstraction;
using SecureP.Service.Abstraction;
using SecureP.Service.Abstraction.Entities;
using SecureP.Service.ParkingLocationService.Mappers;

namespace SecureP.Service.ParkingLocationService;

public class ParkingLocationService<TKey> : IParkingLocationService<TKey>
    where TKey : IEquatable<TKey>
{
    private readonly ILogger<ParkingLocationService<TKey>> _logger;
    private readonly IParkingLocationRepository<TKey> _parkingLocationRepository;

    public ParkingLocationService(ILogger<ParkingLocationService<TKey>> logger,
        IParkingLocationRepository<TKey> parkingLocationRepository)
    {
        _parkingLocationRepository = parkingLocationRepository;
        _logger = logger;
    }

    public Task<(ValidationResult, ParkingLocation<TKey>?)> CreateParkingLocationAsync(CreateParkingLocationRequest request)
    {
        return _parkingLocationRepository.CreateParkingLocationAsync(request.ToCreateParkingLocationDto());
    }

    public Task<bool> DeleteParkingLocationAsync(TKey id)
    {
        return _parkingLocationRepository.DeleteParkingLocationAsync(id);
    }

    public Task<GetParkingLocationDto<TKey>?> GetParkingLocationByIdAsync(TKey id)
    {
        return _parkingLocationRepository.GetParkingLocationByIdAsync(id);
    }

    public Task<GetAllParkingLocationsDto<TKey>?> GetParkingLocationsAsync(int page, int limit, ParkingLocationOrderBy sort, bool desc, string? search = null)
    {
        return _parkingLocationRepository.GetParkingLocationsAsync(page, limit, sort, desc, search);
    }

    public Task<ValidationResult> UpdateParkingLocationAsync(TKey id, UpdateParkingLocationRequest request)
    {
        return _parkingLocationRepository.UpdateParkingLocationAsync(id, request.ToUpdateParkingLocationDto());
    }
}
