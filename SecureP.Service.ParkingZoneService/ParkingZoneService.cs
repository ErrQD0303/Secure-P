using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SecureP.Identity.Models;
using SecureP.Identity.Models.Dto;
using SecureP.Identity.Models.Dto.SortModels;
using SecureP.Identity.Models.Result;
using SecureP.Repository.Abstraction;
using SecureP.Repository.Abstraction.Models;
using SecureP.Service.Abstraction;
using SecureP.Service.ParkingZoneService.Mappers;

namespace SecureP.Service.ParkingZoneService;

public class ParkingZoneService<TKey> : IParkingZoneService<TKey> where TKey : IEquatable<TKey>
{
    private ILogger<ParkingZoneService<TKey>> Logger { get; }
    private IParkingZoneRepository<TKey> ParkingZoneRepository { get; }

    public ParkingZoneService(ILogger<ParkingZoneService<TKey>> logger, IParkingZoneRepository<TKey> parkingZoneRepository)
    {
        Logger = logger;
        ParkingZoneRepository = parkingZoneRepository;
    }

    public async Task<(ValidationResult, CreatedParkingZoneDto<TKey>?)> CreateParkingZoneAsync(CreateParkingZoneDto<TKey> createParkingZoneDto)
    {
        if (!ValidateParkingZoneModel(createParkingZoneDto.ToParkingZoneValidationModel(), out var validationResult))
        {
            Logger.LogError($"{nameof(CreateParkingZoneAsync)}: Validation failed for parking zone model.");
            return (validationResult, null);
        }

        await ParkingZoneRepository.AddAsync(createParkingZoneDto.ToParkingZone());

        try
        {
            var result = await ParkingZoneRepository.SaveChangesAsync();
            if (result <= 0)
            {
                Logger.LogError($"{nameof(CreateParkingZoneAsync)}: Failed to save changes to the database.");
                return (new ValidationResult { Success = false, Message = "Failed to save changes to the database." }, null);
            }

            Logger.LogInformation($"{nameof(CreateParkingZoneAsync)}: Successfully created parking zone with ID {createParkingZoneDto.Id}.");
            var createdParkingZone = await ParkingZoneRepository.GetByIdAsync(createParkingZoneDto.Id);

            if (createdParkingZone is null)
            {
                Logger.LogError($"{nameof(CreateParkingZoneAsync)}: Created parking zone with ID {createParkingZoneDto.Id} not found.");
                return (new ValidationResult { Success = false, Message = "Created parking zone not found." }, null);
            }

            return (new ValidationResult { Success = true, Message = "Parking zone created successfully" }, createdParkingZone.ToCreatedParkingZoneDto());
        }
        catch (DbUpdateException)
        {
            Logger.LogError($"{nameof(CreateParkingZoneAsync)}: Database update exception occurred.");
            return (new ValidationResult { Success = false, Message = "Database update exception occurred." }, null);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{nameof(CreateParkingZoneAsync)}: An unexpected error occurred: {ex.Message}");
            return (new ValidationResult { Success = false, Message = "An unexpected error occurred." }, null);
        }
    }

    public async Task<bool> DeleteParkingZoneAsync(TKey id)
    {
        if (id is null || id.Equals(default))
        {
            Logger.LogError($"{nameof(DeleteParkingZoneAsync)}: Id cannot be null or default.");
            return false;
        }

        var existingParkingZone = await ParkingZoneRepository.GetByIdAsync(id);
        if (existingParkingZone is null)
        {
            Logger.LogError($"{nameof(DeleteParkingZoneAsync)}: Parking zone with ID {id} not found.");
            return false;
        }

        await ParkingZoneRepository.DeleteAsync(existingParkingZone);
        try
        {
            var result = await ParkingZoneRepository.SaveChangesAsync();
            if (result <= 0)
            {
                Logger.LogError($"{nameof(DeleteParkingZoneAsync)}: Failed to save changes to the database.");
                return false;
            }

            Logger.LogInformation($"{nameof(DeleteParkingZoneAsync)}: Successfully deleted parking zone with ID {id}.");
            return true;
        }
        catch (DbUpdateException ex)
        {
            Logger.LogError($"{nameof(DeleteParkingZoneAsync)}: Database update exception occurred: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{nameof(DeleteParkingZoneAsync)}: An unexpected error occurred: {ex.Message}");
            return false;
        }
    }

    public async Task<GetParkingZoneDto<TKey>?> GetParkingZoneByIdAsync(TKey id)
    {
        if (id is null || id.Equals(default))
        {
            Logger.LogError($"{nameof(GetParkingZoneByIdAsync)}: Id cannot be null or default.");
            return null;
        }

        Logger.LogInformation($"{nameof(GetParkingZoneByIdAsync)}: Fetching parking zone with ID {id}.");
        return (await ParkingZoneRepository.GetByIdAsync(id))?.ToGetParkingZoneDto();
    }

    public async Task<GetAllParkingZoneDto<TKey>?> GetParkingZonesAsync(int page = 1, int limit = -1, ParkingZoneOrderBy sort = ParkingZoneOrderBy.Name, bool desc = false, string? search = null)
    {
        Logger.LogInformation($"{nameof(GetParkingZonesAsync)}: Fetching parking zones. Page: {page}, Limit: {limit}, Sort: {sort}, Desc: {desc}, Search: {search}.");
        page = page < 1 ? 0 : page - 1;
        var totalItems = await ParkingZoneRepository.CountAsync();
        if (limit < 1)
        {
            limit = totalItems;
        }

        var totalPages = (int)Math.Ceiling((double)totalItems / limit);
        var items = await ParkingZoneRepository.GetAllAsync(page, limit, sort, desc, search);

        return new GetAllParkingZoneDto<TKey>
        {
            Items = [.. items.Select(x => x.ToGetParkingZoneDto())],
            TotalItems = totalItems,
            TotalPages = totalPages,
        };
    }

    public async Task<ValidationResult> UpdateParkingZoneAsync(UpdateParkingZoneDto<TKey> updateParkingZoneDto)
    {
        if (!ValidateParkingZoneModel(updateParkingZoneDto.ToParkingZoneValidationModel(), out var validationResult))
        {
            Logger.LogError($"{nameof(UpdateParkingZoneAsync)}: Validation failed for parking zone model.");
            return validationResult;
        }

        var existingParkingZone = await ParkingZoneRepository.GetByIdAsync(updateParkingZoneDto.Id);
        if (existingParkingZone is null)
        {
            Logger.LogWarning($"{nameof(UpdateParkingZoneAsync)}: Parking zone with ID {existingParkingZone!.Id} not found.");
            return new ValidationResult { Success = false, Message = "Parking zone not found." };
        }

        try
        {
            UpdateExistingParkingZone(existingParkingZone, updateParkingZoneDto);
            var result = await ParkingZoneRepository.SaveChangesAsync();
            if (result <= 0)
            {
                Logger.LogError($"{nameof(UpdateParkingZoneAsync)}: Failed to save changes to the database.");
                return new ValidationResult { Success = false, Message = "Failed to save changes to the database." };
            }

            Logger.LogInformation($"{nameof(UpdateParkingZoneAsync)}: Parking zone with ID {existingParkingZone.Id} updated successfully.");
            return new ValidationResult { Success = true, Message = "Parking zone updated successfully." };
        }
        catch (DbUpdateConcurrencyException ex)
        {
            Logger.LogError(ex, $"{nameof(UpdateParkingZoneAsync)}: Concurrency exception occurred: {ex.Message}");
            return new ValidationResult { Success = false, Message = "Concurrency exception occurred. Someone else has change the record before you." };
        }
        catch (DbUpdateException ex)
        {
            Logger.LogError(ex, $"{nameof(UpdateParkingZoneAsync)}: Database update exception occurred: {ex.Message}");
            return new ValidationResult { Success = false, Message = "Database update exception occurred." };
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"{nameof(UpdateParkingZoneAsync)}: An unexpected error occurred: {ex.Message}");
            return new ValidationResult { Success = false, Message = "An unexpected error occurred." };
        }
    }


    private static bool ValidateParkingZoneModel(ParkingZoneValidationModel<TKey> validationModel, out ValidationResult validationResult)
    {
        validationResult = new ValidationResult { Success = true };

        if (validationModel.Id is null || validationModel.Id.Equals(default))
        {
            validationResult.Success = false;
            validationResult.Errors ??= [];
            validationResult.Errors.Add("id", "Id cannot be null or default.");
        }

        if (string.IsNullOrEmpty(validationModel.Name))
        {
            validationResult.Success = false;
            validationResult.Errors ??= [];
            validationResult.Errors.Add("name", "Name cannot be null or empty.");
        }

        if (validationModel.Capacity < 0)
        {
            validationResult.Success = false;
            validationResult.Errors ??= [];
            validationResult.Errors.Add("capacity", "Capacity must be greater than or equal to 0.");
        }

        if (validationModel.AvailableSpaces < 0)
        {
            validationResult.Success = false;
            validationResult.Errors ??= [];
            validationResult.Errors.Add("available_spaces", "Available Spaces must be greater than or equal to 0.");
        }

        if (validationModel.AvailableSpaces > validationModel.Capacity)
        {
            validationResult.Success = false;
            validationResult.Errors ??= [];
            validationResult.Errors.Add("available_spaces", "Available Spaces cannot be greater than Capacity.");
        }

        return validationResult.Success;
    }

    private void UpdateExistingParkingZone(ParkingZone<TKey> existingParkingZone, UpdateParkingZoneDto<TKey> parkingZone)
    {
        if (existingParkingZone.ConcurrencyStamp != parkingZone.ConcurrencyStamp)
        {
            throw new DbUpdateConcurrencyException($"Concurrency stamp mismatch. Expected: {existingParkingZone.ConcurrencyStamp}, Actual: {parkingZone.ConcurrencyStamp}");
        }

        if (string.IsNullOrEmpty(parkingZone.Name) && parkingZone.Name != existingParkingZone.Name)
        {
            existingParkingZone.Name = parkingZone.Name;
        }

        if (parkingZone.Capacity != existingParkingZone.Capacity)
        {
            existingParkingZone.Capacity = parkingZone.Capacity;
        }

        if (parkingZone.AvailableSpaces != existingParkingZone.AvailableSpaces)
        {
            existingParkingZone.AvailableSpaces = parkingZone.AvailableSpaces;
        }

        existingParkingZone.ConcurrencyStamp = Guid.NewGuid().ToString();
    }
}
