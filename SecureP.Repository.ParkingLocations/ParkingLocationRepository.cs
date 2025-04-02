using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SecureP.Data;
using SecureP.Identity.Models;
using SecureP.Identity.Models.Dto;
using SecureP.Identity.Models.Dto.SortModels;
using SecureP.Identity.Models.Result;
using SecureP.Repository.Abstraction;
using SecureP.Repository.Abstraction.Models;
using SecureP.Repository.ParkingLocations.Mappers;

namespace SecureP.Repository.ParkingLocations;

public class ParkingLocationRepository<TKey> : IParkingLocationRepository<TKey> where TKey : IEquatable<TKey>
{
    private readonly AppDbContext<TKey> _context;
    private readonly ILogger<ParkingLocationRepository<TKey>> _logger;

    public ParkingLocationRepository(ILogger<ParkingLocationRepository<TKey>> logger, AppDbContext<TKey> dbContext)
    {
        _context = dbContext;
        _logger = logger;
    }

    public async Task<(ValidationResult, ParkingLocation<TKey>?)> CreateParkingLocationAsync(CreateParkingLocationDto parkingLocation)
    {
        if (!ValidateParkingLocationModel(parkingLocation.ToParkingLocationValidationModel(), out var validationResult))
        {
            _logger.LogError("CreateParkingLocationAsync: Validation failed for parking location model.");
            return (validationResult, null);
        }

        var newParkingLocation = new ParkingLocation<TKey>
        {
            Id = typeof(TKey) switch
            {
                Type t when t == typeof(Guid) => (TKey)(object)Guid.NewGuid(),
                Type t when t == typeof(string) => (TKey)(object)Guid.NewGuid().ToString(),
                _ => throw new InvalidOperationException("Invalid type for TKey")
            },
            Name = parkingLocation.Name,
            Address = parkingLocation.Address,
            Capacity = parkingLocation.Capacity,
            AvailableSpaces = parkingLocation.AvailableSpaces,
            ParkingRate = new ParkingRate<TKey>
            {
                HourlyRate = parkingLocation.ParkingRate?.HourlyRate ?? 0,
                DailyRate = parkingLocation.ParkingRate?.DailyRate ?? 0,
                MonthlyRate = parkingLocation.ParkingRate?.MonthlyRate ?? 0
            },
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        await _context.ParkingLocations.AddAsync(newParkingLocation);

        try
        {
            var result = await _context.SaveChangesAsync();

            if (result <= 0)
            {
                validationResult.Message = "Failed to create parking location.";
                _logger.LogError("CreateParkingLocationAsync: Failed to save changes to the database.");

                return (validationResult, null);
            }

            validationResult.Success = true;
            validationResult.Message = "Parking location created successfully.";
            _logger.LogInformation("CreateParkingLocationAsync: Parking location created successfully.");
            return (validationResult, newParkingLocation);
        }
        catch (DbUpdateException ex)
        {
            validationResult.Success = false;
            validationResult.Message = "Failed to create parking location due to database error.";
            _logger.LogError(ex, "CreateParkingLocationAsync: Database error occurred while creating parking location.");
            return (validationResult, null);
        }
    }

    private static bool ValidateParkingLocationModel(ParkingLocationValidationModel model, out ValidationResult validationResult)
    {
        validationResult = new ValidationResult { Success = true, Errors = [] };

        if (string.IsNullOrWhiteSpace(model.Name))
        {
            validationResult.Success = false;
            validationResult.Errors.Add("Name", "Parking location name is required.");
        }

        if (string.IsNullOrWhiteSpace(model.Address))
        {
            validationResult.Success = false;
            validationResult.Errors.Add("Address", "Parking location address is required.");
        }

        if (model.Capacity <= 0)
        {
            validationResult.Success = false;
            validationResult.Errors.Add("Capacity", "Parking location capacity must be greater than zero.");
        }

        if (model.AvailableSpaces < 0)
        {
            validationResult.Success = false;
            validationResult.Errors.Add("AvailableSpaces", "Parking location available spaces cannot be negative.");
        }

        return validationResult.Success;
    }

    public async Task<bool> DeleteParkingLocationAsync(TKey id)
    {
        _logger.LogInformation($"DeleteParkingLocationAsync: Deleting parking location with ID {id}.");
        if (id != null && id is string stringId)
        {
            id = (TKey)(object)stringId.Trim();
        }
        var parkingLocation = await _context.ParkingLocations.FirstOrDefaultAsync(p => p.Id.Equals(id));

        if (parkingLocation == null)
        {
            _logger.LogWarning($"DeleteParkingLocationAsync: Parking location with ID {id} not found.");
            return false;
        }

        _context.ParkingLocations.Remove(parkingLocation);
        return (await _context.SaveChangesAsync()) > 0;
    }

    public async Task<GetParkingLocationDto<TKey>?> GetParkingLocationByIdAsync(TKey id)
    {
        _logger.LogInformation($"GetParkingLocationByIdAsync: Fetching parking location with ID {id}.");
        if (id != null && id is string stringId)
        {
            id = (TKey)(object)stringId.Trim();
        }
        var parkingLocation = await _context.ParkingLocations.Include(pl => pl.ParkingRate).FirstOrDefaultAsync(p => p.Id.Equals(id));

        if (parkingLocation == null)
        {
            _logger.LogWarning($"GetParkingLocationByIdAsync: Parking location with ID {id} not found.");
            return null;
        }

        _logger.LogInformation($"GetParkingLocationByIdAsync: Parking location with ID {id} found.");

        return parkingLocation.ToGetParkingLocationDto();
    }

    public async Task<GetAllParkingLocationsDto<TKey>?> GetParkingLocationsAsync(int pageIndex = 1, int pageSize = 10, ParkingLocationOrderBy orderBy = ParkingLocationOrderBy.Name, bool desc = false)
    {
        _logger.LogInformation($"GetParkingLocationsAsync: Fetching all parking locations with pagination. PageIndex: {pageIndex}, PageSize: {pageSize}.");

        pageIndex = pageIndex < 1 ? 0 : pageIndex - 1; // Adjust for zero-based index

        if (pageIndex < 0 || pageSize <= 0)
        {
            _logger.LogWarning("GetParkingLocationsAsync: Invalid pagination parameters.");
            return null;
        }

        var totalCount = await _context.ParkingLocations.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        if (pageIndex >= totalPages)
        {
            _logger.LogWarning($"GetParkingLocationsAsync: Page index {pageIndex} exceeds total pages {totalPages}.");
            return null;
        }

        var parkingLocations = _context.ParkingLocations
            .Include(pl => pl.ParkingRate)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .AsNoTracking();

        switch (orderBy)
        {
            case ParkingLocationOrderBy.Name:
                parkingLocations = desc ? parkingLocations.OrderByDescending(p => p.Name) : parkingLocations.OrderBy(p => p.Name);
                break;
            case ParkingLocationOrderBy.Address:
                parkingLocations = desc ? parkingLocations.OrderByDescending(p => p.Address) : parkingLocations.OrderBy(p => p.Address);
                break;
            case ParkingLocationOrderBy.Capacity:
                parkingLocations = desc ? parkingLocations.OrderByDescending(p => p.Capacity) : parkingLocations.OrderBy(p => p.Capacity);
                break;
            case ParkingLocationOrderBy.AvailableSpaces:
                parkingLocations = desc ? parkingLocations.OrderByDescending(p => p.AvailableSpaces) : parkingLocations.OrderBy(p => p.AvailableSpaces);
                break;
            default:
                _logger.LogWarning($"GetParkingLocationsAsync: Invalid order by parameter '{orderBy}'. Defaulting to 'name'.");
                parkingLocations = desc ? parkingLocations.OrderByDescending(p => p.Name) : parkingLocations.OrderBy(p => p.Name);
                break;
        }

        return new GetAllParkingLocationsDto<TKey>
        {
            Items = await parkingLocations
            .Select(p => new GetParkingLocationDto<TKey>
            {
                Id = p.Id,
                Name = p.Name,
                Address = p.Address,
                Capacity = p.Capacity,
                AvailableSpaces = p.AvailableSpaces,
                HourlyRate = p.ParkingRate != null ? p.ParkingRate.HourlyRate : 0,
                DailyRate = p.ParkingRate != null ? p.ParkingRate.DailyRate : 0,
                MonthlyRate = p.ParkingRate != null ? p.ParkingRate.MonthlyRate : 0,
                ConcurrencyStamp = p.ConcurrencyStamp ?? string.Empty
            }).ToListAsync()
        };
    }

    public async Task<ValidationResult> UpdateParkingLocationAsync(TKey id, UpdateParkingLocationDto parkingLocation)
    {
        var validationResult = new ValidationResult { Success = true, Errors = [] };

        if (id == null)
        {
            validationResult.Success = false;
            validationResult.Errors.Add("id", "Parking location ID is required.");
        }

        if (!ValidateParkingLocationModel(parkingLocation.ToParkingLocationValidationModel(), out validationResult))
        {
            _logger.LogError("UpdateParkingLocationAsync: Validation failed for parking location model.");
            return validationResult;
        }

        if (!validationResult.Success)
        {
            validationResult.Message = "Validation failed for parking location model.";
            _logger.LogError("UpdateParkingLocationAsync: Validation failed for parking location model.");
            return validationResult;
        }

        if (id != null && id is string stringId)
        {
            id = (TKey)(object)stringId.Trim();
        }

        var existingParkingLocation = await _context.ParkingLocations.Include(pl => pl.ParkingRate).FirstOrDefaultAsync(p => p.Id.Equals(id));

        if (existingParkingLocation is null)
        {
            validationResult.Success = false;
            validationResult.Message = "Parking location not found.";
            _logger.LogWarning($"UpdateParkingLocationAsync: Parking location with ID {id} not found.");

            return validationResult;
        }

        try
        {
            var result = await UpdateExistingParkingLocation(existingParkingLocation, parkingLocation, _context);

            if (result <= 0)
            {
                validationResult.Success = false;
                validationResult.Message = "Failed to update parking location.";
                _logger.LogError($"UpdateParkingLocationAsync: Failed to update parking location with ID {id}.");

                return validationResult;
            }

            validationResult.Success = true;
            validationResult.Message = "Parking location updated successfully.";
            _logger.LogInformation($"UpdateParkingLocationAsync: Parking location with ID {id} updated successfully.");
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError($"UpdateParkingLocationAsync: Concurrency conflict during update with error: {ex.Message}");
            validationResult.Success = false;
            validationResult.Message = "Concurrency conflict occurred while updating parking location.";
            validationResult.Errors?.Add("concurrency_stamp", "Concurrency conflict occurred. Please try again.");
        }

        return validationResult;
    }

    private static async Task<int> UpdateExistingParkingLocation(ParkingLocation<TKey> existingParkingLocation, UpdateParkingLocationDto parkingLocation, AppDbContext<TKey> context)
    {
        if (!string.Equals(existingParkingLocation.ConcurrencyStamp, parkingLocation.ConcurrencyStamp, StringComparison.Ordinal))
        {
            throw new DbUpdateConcurrencyException("Concurrency conflict occurred. The parking location has been modified by another process.");
        }

        existingParkingLocation.Name = parkingLocation.Name ?? existingParkingLocation.Name;
        existingParkingLocation.Address = parkingLocation.Address ?? existingParkingLocation.Address;
        existingParkingLocation.Capacity = parkingLocation.Capacity ?? existingParkingLocation.Capacity;
        existingParkingLocation.AvailableSpaces = parkingLocation.AvailableSpaces ?? existingParkingLocation.AvailableSpaces;
        existingParkingLocation.ParkingRate ??= new();
        existingParkingLocation.ParkingRate.ParkingLocationId = existingParkingLocation.Id;
        existingParkingLocation.ParkingRate.HourlyRate = parkingLocation.ParkingRate?.HourlyRate ?? existingParkingLocation.ParkingRate.HourlyRate;
        existingParkingLocation.ParkingRate.DailyRate = parkingLocation.ParkingRate?.DailyRate ?? existingParkingLocation.ParkingRate.DailyRate;
        existingParkingLocation.ParkingRate.MonthlyRate = parkingLocation.ParkingRate?.MonthlyRate ?? existingParkingLocation.ParkingRate.MonthlyRate;
        existingParkingLocation.ConcurrencyStamp = Guid.NewGuid().ToString();

        context.ParkingLocations.Update(existingParkingLocation);
        return await context.SaveChangesAsync();
    }
}