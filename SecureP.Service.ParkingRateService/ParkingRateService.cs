using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SecureP.Identity.Models;
using SecureP.Identity.Models.Dto;
using SecureP.Identity.Models.Dto.SortModels;
using SecureP.Identity.Models.Result;
using SecureP.Repository.Abstraction;
using SecureP.Repository.Abstraction.Models;
using SecureP.Service.Abstraction;
using SecureP.Service.ParkingRateService.Mappers;

namespace SecureP.Service.ParkingRateService;

public class ParkingRateService<TKey> : IParkingRateService<TKey> where TKey : IEquatable<TKey>
{
    private readonly ILogger<ParkingRateService<TKey>> _logger;
    private readonly IParkingRateRepository<TKey> _parkingRateRepository;

    public ParkingRateService(ILogger<ParkingRateService<TKey>> logger, IParkingRateRepository<TKey> parkingRateRepository)
    {
        _logger = logger;
        _parkingRateRepository = parkingRateRepository;
    }
    public async Task<(ValidationResult, CreatedParkingRateDto<TKey>?)> CreateParkingRateAsync(CreateParkingRateDto<TKey> newParkingRateDto)
    {
        if (!ValidateParkingRateModel(newParkingRateDto.ToParkingRateValidationModel(), out var validationResult))
        {
            _logger.LogError($"{nameof(CreateParkingRateAsync)}: Validation failed for parking rate model.");
            return (validationResult, null);
        }

        await _parkingRateRepository.AddAsync(newParkingRateDto.ToParkingRate());

        try
        {
            var result = await _parkingRateRepository.SaveChangesAsync();
            if (result <= 0)
            {
                _logger.LogError($"{nameof(CreateParkingRateAsync)}: Failed to save changes to the database.");
                return (new ValidationResult { Success = false, Message = "Failed to save changes to the database." }, null);
            }

            _logger.LogInformation($"{nameof(CreateParkingRateAsync)}: Parking rate created successfully with ID {newParkingRateDto.Id}.");
            var createdParkingRate = await _parkingRateRepository.GetByIdAsync(newParkingRateDto.Id!);

            if (createdParkingRate is null)
            {
                _logger.LogError($"{nameof(CreateParkingRateAsync)}: Created parking rate not found in the database.");
                return (new ValidationResult { Success = false, Message = "Created parking rate not found in the database." }, null);
            }

            return (new ValidationResult
            {
                Success = true,
                Message = "Parking rate created successfully."
            }, createdParkingRate.ToCreatedParkingRateDto());
        }
        catch (DbUpdateException)
        {
            _logger.LogError($"{nameof(CreateParkingRateAsync)}: Database update exception occurred.");
            return (new ValidationResult { Success = false, Message = "Database update exception occurred." }, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(CreateParkingRateAsync)}: An unexpected error occurred: {ex.Message}");
            return (new ValidationResult { Success = false, Message = "An unexpected error occurred." }, null);
        }
    }

    public async Task<bool> DeleteParkingRateAsync(TKey id)
    {
        if (id is null || id.Equals(default))
        {
            _logger.LogError($"{nameof(DeleteParkingRateAsync)}: Id cannot be null or default.");
            return false;
        }

        var existingParkingRate = await _parkingRateRepository.GetByIdAsync(id);
        if (existingParkingRate is null)
        {
            _logger.LogWarning($"{nameof(DeleteParkingRateAsync)}: Parking rate with ID {id} not found.");
            return false;
        }

        await _parkingRateRepository.DeleteAsync(existingParkingRate);
        try
        {
            var result = await _parkingRateRepository.SaveChangesAsync();
            if (result <= 0)
            {
                _logger.LogError($"{nameof(DeleteParkingRateAsync)}: Failed to save changes to the database.");
                return false;
            }

            _logger.LogInformation($"{nameof(DeleteParkingRateAsync)}: Parking rate with ID {id} deleted successfully.");
            return true;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, $"{nameof(DeleteParkingRateAsync)}: Database update exception occurred: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(DeleteParkingRateAsync)}: An unexpected error occurred: {ex.Message}");
        }

        return false;
    }

    public async Task<GetParkingRateDto<TKey>?> GetParkingRateByIdAsync(TKey id)
    {
        if (id is null || id.Equals(default))
        {
            _logger.LogError($"{nameof(GetParkingRateByIdAsync)}: Id cannot be null or default.");
            return null;
        }

        _logger.LogInformation($"{nameof(GetParkingRateByIdAsync)}: Fetching parking rate with ID {id}.");
        return (await _parkingRateRepository.GetByIdAsync(id))?.ToGetParkingRateDto();
    }

    public async Task<GetAllParkingRateDto<TKey>?> GetParkingRatesAsync(int page, int limit, ParkingRateOrderBy sort, bool desc, string? search = null)
    {
        _logger.LogInformation($"{nameof(GetParkingRatesAsync)}: Fetching parking rates. Page: {page}, Limit: {limit}, Sort: {sort}, Desc: {desc}, Search: {search}.");
        page = page < 1 ? 0 : page - 1;
        var totalItems = await _parkingRateRepository.CountAsync();
        if (limit < 1)
        {
            limit = totalItems;
        }

        var totalPages = (int)Math.Ceiling((double)totalItems / limit);
        var items = await _parkingRateRepository.GetAllAsync(page, limit, sort, desc, search);

        return new GetAllParkingRateDto<TKey>
        {
            Items = [.. items.Select(x => x.ToGetParkingRateDto())],
            TotalItems = totalItems,
            TotalPages = totalPages,
        };
    }

    public async Task<ValidationResult> UpdateParkingRateAsync(UpdateParkingRateDto<TKey> updateParkingRateDto)
    {
        if (!ValidateParkingRateModel(updateParkingRateDto.ToParkingRateValidationModel(), out var validationResult))
        {
            _logger.LogError($"{nameof(UpdateParkingRateAsync)}: Validation failed for parking rate model.");
            return validationResult;
        }

        var existingParkingRate = await _parkingRateRepository.GetByIdAsync(updateParkingRateDto.Id);
        if (existingParkingRate is null)
        {
            _logger.LogWarning($"{nameof(UpdateParkingRateAsync)}: Parking rate with ID {existingParkingRate!.Id} not found.");
            return new ValidationResult { Success = false, Message = "Parking rate not found." };
        }

        try
        {
            UpdateExistingParkingRate(existingParkingRate, updateParkingRateDto);
            var result = await _parkingRateRepository.SaveChangesAsync();
            if (result <= 0)
            {
                _logger.LogError($"{nameof(UpdateParkingRateAsync)}: Failed to save changes to the database.");
                return new ValidationResult { Success = false, Message = "Failed to save changes to the database." };
            }

            _logger.LogInformation($"{nameof(UpdateParkingRateAsync)}: Parking rate with ID {existingParkingRate.Id} updated successfully.");
            return new ValidationResult { Success = true, Message = "Parking rate updated successfully." };
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, $"{nameof(UpdateParkingRateAsync)}: Concurrency exception occurred: {ex.Message}");
            return new ValidationResult { Success = false, Message = "Concurrency exception occurred. Someone else has change the record before you." };
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, $"{nameof(UpdateParkingRateAsync)}: Database update exception occurred: {ex.Message}");
            return new ValidationResult { Success = false, Message = "Database update exception occurred." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(UpdateParkingRateAsync)}: An unexpected error occurred: {ex.Message}");
            return new ValidationResult { Success = false, Message = "An unexpected error occurred." };
        }
    }

    private void UpdateExistingParkingRate(ParkingRate<TKey> existingParkingRate, UpdateParkingRateDto<TKey> parkingRate)
    {
        if (existingParkingRate.ConcurrencyStamp != parkingRate.ConcurrencyStamp)
        {
            throw new DbUpdateConcurrencyException($"Concurrency stamp mismatch. Expected: {existingParkingRate.ConcurrencyStamp}, Actual: {parkingRate.ConcurrencyStamp}");
        }

        if (parkingRate.HourlyRate.HasValue && parkingRate.HourlyRate != existingParkingRate.HourlyRate)
        {
            existingParkingRate.HourlyRate = parkingRate.HourlyRate.Value;
        }
        if (parkingRate.DailyRate.HasValue && parkingRate.DailyRate != existingParkingRate.DailyRate)
        {
            existingParkingRate.DailyRate = parkingRate.DailyRate.Value;
        }
        if (parkingRate.MonthlyRate.HasValue && parkingRate.MonthlyRate != existingParkingRate.MonthlyRate)
        {
            existingParkingRate.MonthlyRate = parkingRate.MonthlyRate.Value;
        }

        existingParkingRate.ConcurrencyStamp = Guid.NewGuid().ToString();
    }

    private bool ValidateParkingRateModel(ParkingRateValidationModel<TKey> parkingRateValidationModel, out ValidationResult validationResult)
    {
        validationResult = new ValidationResult { Success = true };

        if (parkingRateValidationModel.Id is null || parkingRateValidationModel.Id.Equals(default))
        {
            validationResult.Success = false;
            validationResult.Errors ??= [];
            validationResult.Errors.Add("id", "Id cannot be null or default.");
        }

        if (parkingRateValidationModel.HourlyRate < 0)
        {
            validationResult.Success = false;
            validationResult.Errors ??= [];
            validationResult.Errors.Add("hourly_rate", "Hourly rate must be greater than or equal to 0.");
        }

        if (parkingRateValidationModel.DailyRate < 0)
        {
            validationResult.Success = false;
            validationResult.Errors ??= [];
            validationResult.Errors.Add("daily_rate", "Daily rate must be greater than or equal to 0.");
        }

        if (parkingRateValidationModel.MonthlyRate < 0)
        {
            validationResult.Success = false;
            validationResult.Errors ??= [];
            validationResult.Errors.Add("monthly_rate", "Monthly rate must be greater than or equal to 0.");
        }

        return validationResult.Success;
    }
}
