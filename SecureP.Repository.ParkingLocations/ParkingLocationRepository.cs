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

    public async Task<(ValidationResult, ParkingLocation<TKey>?)> CreateParkingLocationAsync(CreateParkingLocationDto<TKey> parkingLocation)
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
            ParkingZones = [],
            ParkingLocationRates = [],
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        foreach (var zone in parkingLocation.ParkingZones)
        {
            newParkingLocation.ParkingZones.Add(new ParkingZone<TKey>
            {
                Id = typeof(TKey) switch
                {
                    Type t when t == typeof(Guid) => (TKey)(object)Guid.NewGuid(),
                    Type t when t == typeof(string) => (TKey)(object)Guid.NewGuid().ToString(),
                    _ => throw new InvalidOperationException("Invalid type for TKey")
                },
                Name = zone.Name,
                Capacity = zone.Capacity,
                AvailableSpaces = zone.AvailableSpaces
            });
        }

        newParkingLocation.ParkingLocationRates.Add(new ParkingLocationRate<TKey>
        {
            Id = typeof(TKey) switch
            {
                Type t when t == typeof(Guid) => (TKey)(object)Guid.NewGuid(),
                Type t when t == typeof(string) => (TKey)(object)Guid.NewGuid().ToString(),
                _ => throw new InvalidOperationException("Invalid type for TKey")
            },
            ParkingLocationId = newParkingLocation.Id,
            ParkingRateId = parkingLocation.ParkingRateId!,
            EffectiveFrom = DateTime.UtcNow,
            EffectiveTo = null,
        });

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

    private static bool ValidateParkingLocationModel(ParkingLocationValidationModel<TKey> model, out ValidationResult validationResult)
    {
        validationResult = new ValidationResult { Success = true, Errors = [] };

        if (string.IsNullOrWhiteSpace(model.Name))
        {
            validationResult.Success = false;
            validationResult.Errors.Add("name", "Parking location name is required.");
        }

        if (string.IsNullOrWhiteSpace(model.Address))
        {
            validationResult.Success = false;
            validationResult.Errors.Add("address", "Parking location address is required.");
        }

        if (model.ParkingRateId is null)
        {
            validationResult.Success = false;
            validationResult.Errors.Add("parking_rate_id", "You must specify a parking rate for this parking location.");
        }

        int index = 0;
        foreach (var zone in model.ParkingZones)
        {
            Dictionary<string, string> zoneErrors = [];
            if (string.IsNullOrWhiteSpace(zone.Name))
            {
                validationResult.Success = false;
                zoneErrors.Add("name", $"Parking zone name is required");
            }

            if (zone.Capacity <= 0)
            {
                validationResult.Success = false;
                zoneErrors.Add("capacity", $"Parking zone capacity must be greater than 0.");
            }

            if (zone.AvailableSpaces < 0)
            {
                validationResult.Success = false;
                zoneErrors.Add("available_spaces", $"Parking zone available spaces cannot be negative.");
            }

            if (zoneErrors.Count > 0)
            {
                var newEntryErrors = new Dictionary<string, object>
                {
                    { $"{index}", zoneErrors }
                };
                validationResult.Errors.Add($"parking_zones", newEntryErrors);
            }

            ++index;
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
        var parkingLocation = await _context.ParkingLocations
            .Include(pl => pl.ParkingZones)
            .Include(pl => pl.ParkingLocationRates)
            .ThenInclude(plr => plr.ParkingRate)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id.Equals(id));

        if (parkingLocation == null)
        {
            _logger.LogWarning($"GetParkingLocationByIdAsync: Parking location with ID {id} not found.");
            return null;
        }

        _logger.LogInformation($"GetParkingLocationByIdAsync: Parking location with ID {id} found.");

        return parkingLocation.ToGetParkingLocationDto();
    }

    public async Task<GetAllParkingLocationsDto<TKey>?> GetParkingLocationsAsync(int page = 1, int limit = 10, ParkingLocationOrderBy sort = ParkingLocationOrderBy.Name, bool desc = false, string? search = null)
    {
        _logger.LogInformation($"GetParkingLocationsAsync: Fetching all parking locations with pagination. PageIndex: {page}, PageSize: {limit}.");

        page = page < 1 ? 0 : page - 1; // Adjust for zero-based index

        if (page < 0 || limit <= 0)
        {
            _logger.LogWarning("GetParkingLocationsAsync: Invalid pagination parameters.");
            return null;
        }

        var parkingLocations = _context.ParkingLocations
            .Include(pl => pl.ParkingZones)
            .Include(pl => pl.ParkingLocationRates)
            .ThenInclude(plr => plr.ParkingRate)
            .AsQueryable()
            .Where(p => p.ParkingLocationRates.Any(plr => plr.EffectiveFrom <= DateTime.UtcNow && (plr.EffectiveTo == null || plr.EffectiveTo >= DateTime.UtcNow)))
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            parkingLocations = parkingLocations.Where(p => p.Name.Contains(search) || p.Address.Contains(search));
        }

        switch (sort)
        {
            case ParkingLocationOrderBy.Name:
                parkingLocations = desc ? parkingLocations.OrderByDescending(p => p.Name) : parkingLocations.OrderBy(p => p.Name);
                break;
            case ParkingLocationOrderBy.Address:
                parkingLocations = desc ? parkingLocations.OrderByDescending(p => p.Address) : parkingLocations.OrderBy(p => p.Address);
                break;
            case ParkingLocationOrderBy.Capacity:
                parkingLocations = desc
                    ? parkingLocations.OrderByDescending(p => p.ParkingZones.Sum(z => z.Capacity))
                    : parkingLocations.OrderBy(p => p.ParkingZones.Sum(z => z.Capacity));
                break;
            case ParkingLocationOrderBy.AvailableSpaces:
                parkingLocations = desc
                    ? parkingLocations.OrderByDescending(p => p.ParkingZones.Sum(z => z.AvailableSpaces))
                    : parkingLocations.OrderBy(p => p.ParkingZones.Sum(z => z.AvailableSpaces));
                break;
            case ParkingLocationOrderBy.HourlyRate:
                parkingLocations = desc
                    ? parkingLocations.OrderByDescending(p => p.ParkingLocationRates
                        .Where(plr => plr.EffectiveFrom <= DateTime.UtcNow && (plr.EffectiveTo == null || plr.EffectiveTo >= DateTime.UtcNow))
                        .Select(plr => plr.ParkingRate!.HourlyRate).FirstOrDefault())
                    : parkingLocations.OrderBy(p => p.ParkingLocationRates
                        .Where(plr => plr.EffectiveFrom <= DateTime.UtcNow && (plr.EffectiveTo == null || plr.EffectiveTo >= DateTime.UtcNow))
                        .Select(plr => plr.ParkingRate!.HourlyRate).FirstOrDefault());
                break;
            case ParkingLocationOrderBy.DailyRate:
                parkingLocations = desc
                    ? parkingLocations.OrderByDescending(pl => pl.ParkingLocationRates
                        .Where(plr => plr.EffectiveFrom <= DateTime.UtcNow && (plr.EffectiveTo == null || plr.EffectiveTo >= DateTime.UtcNow))
                        .Select(plr => plr.ParkingRate!.DailyRate).FirstOrDefault())
                    : parkingLocations.OrderBy(pl => pl.ParkingLocationRates
                        .Where(plr => plr.EffectiveFrom <= DateTime.UtcNow && (plr.EffectiveTo == null || plr.EffectiveTo >= DateTime.UtcNow))
                        .Select(plr => plr.ParkingRate!.DailyRate).FirstOrDefault());
                break;
            case ParkingLocationOrderBy.MonthlyRate:
                parkingLocations = desc
                    ? parkingLocations.OrderByDescending(pl => pl.ParkingLocationRates
                        .Where(plr => plr.EffectiveFrom <= DateTime.UtcNow && (plr.EffectiveTo == null || plr.EffectiveTo >= DateTime.UtcNow))
                        .Select(plr => plr.ParkingRate!.MonthlyRate).FirstOrDefault())
                    : parkingLocations.OrderBy(pl => pl.ParkingLocationRates
                        .Where(plr => plr.EffectiveFrom <= DateTime.UtcNow && (plr.EffectiveTo == null || plr.EffectiveTo >= DateTime.UtcNow))
                        .Select(plr => plr.ParkingRate!.MonthlyRate).FirstOrDefault());
                break;
            case ParkingLocationOrderBy.TotalParkingZones:
                parkingLocations = desc ? parkingLocations.OrderByDescending(p => p.ParkingZones.Count) : parkingLocations.OrderBy(p => p.ParkingZones.Count);
                break;
            default:
                _logger.LogWarning($"GetParkingLocationsAsync: Invalid order by parameter '{sort}'. Defaulting to 'name'.");
                parkingLocations = desc ? parkingLocations.OrderByDescending(p => p.Name) : parkingLocations.OrderBy(p => p.Name);
                break;
        }

        var totalParkingLocations = await parkingLocations.CountAsync();

        parkingLocations = parkingLocations.Skip(page * limit).Take(limit);

        var returnObject = new GetAllParkingLocationsDto<TKey>
        {
            Items = await parkingLocations
                .AsNoTracking()
                .AsQueryable()
                .Select(p => new GetParkingLocationDto<TKey>
                {
                    Id = p.Id,
                    Name = p.Name,
                    Address = p.Address,
                    ParkingZones = p.ParkingZones.Select(z => new GetParkingLocationParkingZoneDto<TKey>
                    {
                        Id = z.Id,
                        Name = z.Name,
                        Capacity = z.Capacity,
                        AvailableSpaces = z.AvailableSpaces
                    }).ToList(),
                    ParkingRate = new GetParkingLocationParkingRateDto<TKey>
                    {
                        Id = p.ParkingLocationRates.FirstOrDefault(plr => plr.EffectiveFrom <= DateTime.UtcNow && (plr.EffectiveTo == null || plr.EffectiveTo >= DateTime.UtcNow)) != null
                            ? p.ParkingLocationRates.FirstOrDefault(plr => plr.EffectiveFrom <= DateTime.UtcNow && (plr.EffectiveTo == null || plr.EffectiveTo >= DateTime.UtcNow))!.ParkingRate.Id
                            : default!,
                        HourlyRate = p.ParkingLocationRates.FirstOrDefault(plr => plr.EffectiveFrom <= DateTime.UtcNow && (plr.EffectiveTo == null || plr.EffectiveTo >= DateTime.UtcNow)) != null
                            ? p.ParkingLocationRates.FirstOrDefault(plr => plr.EffectiveFrom <= DateTime.UtcNow && (plr.EffectiveTo == null || plr.EffectiveTo >= DateTime.UtcNow))!.ParkingRate.HourlyRate
                            : default!,
                        DailyRate = p.ParkingLocationRates.FirstOrDefault(plr => plr.EffectiveFrom <= DateTime.UtcNow && (plr.EffectiveTo == null || plr.EffectiveTo >= DateTime.UtcNow)) != null
                            ? p.ParkingLocationRates.FirstOrDefault(plr => plr.EffectiveFrom <= DateTime.UtcNow && (plr.EffectiveTo == null || plr.EffectiveTo >= DateTime.UtcNow))!.ParkingRate.DailyRate
                            : default!,
                        MonthlyRate = p.ParkingLocationRates.FirstOrDefault(plr => plr.EffectiveFrom <= DateTime.UtcNow && (plr.EffectiveTo == null || plr.EffectiveTo >= DateTime.UtcNow)) != null
                            ? p.ParkingLocationRates.FirstOrDefault(plr => plr.EffectiveFrom <= DateTime.UtcNow && (plr.EffectiveTo == null || plr.EffectiveTo >= DateTime.UtcNow))!.ParkingRate.MonthlyRate
                            : default!,
                    },
                    ConcurrencyStamp = p.ConcurrencyStamp ?? string.Empty
                }).ToListAsync(),
            TotalItems = totalParkingLocations,
        };

        returnObject.TotalPages = (int)Math.Ceiling((double)returnObject.TotalItems / limit);

        if (page >= returnObject.TotalPages)
        {
            _logger.LogWarning($"GetParkingLocationsAsync: Page index {page} exceeds total pages {returnObject.TotalPages}.");
            return null;
        }

        return returnObject;
    }

    public async Task<ValidationResult> UpdateParkingLocationAsync(TKey id, UpdateParkingLocationDto<TKey> parkingLocation)
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

        var existingParkingLocation = await _context.ParkingLocations
            .Include(pl => pl.ParkingLocationRates)
            .ThenInclude(plr => plr.ParkingRate)
            .Include(pl => pl.ParkingZones)
            .FirstOrDefaultAsync(p => p.Id.Equals(id));

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

    private static async Task<int> UpdateExistingParkingLocation(ParkingLocation<TKey> existingParkingLocation, UpdateParkingLocationDto<TKey> parkingLocation, AppDbContext<TKey> context)
    {
        if (!string.Equals(existingParkingLocation.ConcurrencyStamp, parkingLocation.ConcurrencyStamp, StringComparison.Ordinal))
        {
            throw new DbUpdateConcurrencyException("Concurrency conflict occurred. The parking location has been modified by another process.");
        }

        existingParkingLocation.Name = parkingLocation.Name ?? existingParkingLocation.Name;
        existingParkingLocation.Address = parkingLocation.Address ?? existingParkingLocation.Address;
        existingParkingLocation.ParkingZones = [.. existingParkingLocation.ParkingZones.Where(pz => parkingLocation.ParkingZones.Any(z => z.Id.Equals(pz.Id)))];
        foreach (var existingZone in existingParkingLocation.ParkingZones)
        {
            var updatedZone = parkingLocation.ParkingZones.FirstOrDefault(z => z.Id.Equals(existingZone.Id));
            if (updatedZone != null)
            {
                existingZone.Name = updatedZone.Name ?? existingZone.Name;
                existingZone.Capacity = updatedZone.Capacity > 0 ? updatedZone.Capacity : existingZone.Capacity;
                existingZone.AvailableSpaces = updatedZone.AvailableSpaces >= 0 ? updatedZone.AvailableSpaces : existingZone.AvailableSpaces;
            }
        }
        var currentParkingRate = existingParkingLocation.ParkingLocationRates.FirstOrDefault(plr => plr.EffectiveFrom <= DateTime.UtcNow && (plr.EffectiveTo == null || plr.EffectiveTo >= DateTime.UtcNow));
        if (currentParkingRate != null)
        {
            currentParkingRate.ParkingRateId = parkingLocation.ParkingRateId ?? currentParkingRate.ParkingRateId;
        }
        existingParkingLocation.ConcurrencyStamp = Guid.NewGuid().ToString();

        context.ParkingLocations.Update(existingParkingLocation);
        return await context.SaveChangesAsync();
    }
}