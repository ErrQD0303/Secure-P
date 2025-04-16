using SecureP.Identity.Models.Dto;
using SecureP.Identity.Models.Dto.SortModels;
using SecureP.Identity.Models.Result;
using SecureP.Service.Abstraction.Entities;

namespace SecureP.Service.Abstraction;

public interface IParkingRateService<TKey> where TKey : IEquatable<TKey>
{
    Task<GetParkingRateDto<TKey>?> GetParkingRateByIdAsync(TKey id);
    Task<GetAllParkingRateDto<TKey>?> GetParkingRatesAsync(int page = 1, int limit = -1, ParkingRateOrderBy sort = ParkingRateOrderBy.HourlyRate, bool desc = false, string? search = null);
    Task<(ValidationResult, CreatedParkingRateDto<TKey>?)> CreateParkingRateAsync(CreateParkingRateDto<TKey> request);
    Task<ValidationResult> UpdateParkingRateAsync(UpdateParkingRateDto<TKey> request);
    Task<bool> DeleteParkingRateAsync(TKey id);
}