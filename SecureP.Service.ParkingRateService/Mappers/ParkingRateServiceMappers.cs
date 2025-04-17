using SecureP.Identity.Models;
using SecureP.Identity.Models.Dto;
using SecureP.Repository.Abstraction.Models;
using SecureP.Service.Abstraction.Entities;

namespace SecureP.Service.ParkingRateService.Mappers;

public static class ParkingRateServiceMappers
{
    public static CreateParkingRateDto<TKey> ToCreateParkingRateDto<TKey>(this CreateParkingRateRequest model) where TKey : IEquatable<TKey>
    {
        return new CreateParkingRateDto<TKey>
        {
            Id = typeof(TKey) switch
            {
                Type t when t == typeof(Guid) => (TKey)(object)Guid.NewGuid(),
                Type t when t == typeof(string) => (TKey)(object)Guid.NewGuid().ToString(),
                _ => throw new InvalidOperationException($"Unsupported type: {typeof(TKey)}")
            },
            HourlyRate = model.HourlyRate,
            DailyRate = model.DailyRate,
            MonthlyRate = model.MonthlyRate,
        };
    }

    public static ParkingRateValidationModel<TKey> ToParkingRateValidationModel<TKey>(this CreateParkingRateDto<TKey> model) where TKey : IEquatable<TKey>
    {
        return new ParkingRateValidationModel<TKey>
        {
            Id = model.Id,
            HourlyRate = model.HourlyRate,
            DailyRate = model.DailyRate,
            MonthlyRate = model.MonthlyRate,
        };
    }

    public static ParkingRateValidationModel<TKey> ToParkingRateValidationModel<TKey>(this UpdateParkingRateDto<TKey> model) where TKey : IEquatable<TKey>
    {
        return new ParkingRateValidationModel<TKey>
        {
            Id = model.Id,
            HourlyRate = model.HourlyRate,
            DailyRate = model.DailyRate,
            MonthlyRate = model.MonthlyRate,
        };
    }

    public static GetParkingRateDto<TKey> ToGetParkingRateDto<TKey>(this ParkingRate<TKey> model) where TKey : IEquatable<TKey>
    {
        return new GetParkingRateDto<TKey>
        {
            Id = model.Id,
            HourlyRate = model.HourlyRate,
            DailyRate = model.DailyRate,
            MonthlyRate = model.MonthlyRate,
            ConcurrencyStamp = model.ConcurrencyStamp,
        };
    }

    public static ParkingRate<TKey> ToParkingRate<TKey>(this CreateParkingRateDto<TKey> model) where TKey : IEquatable<TKey>
    {
        return new ParkingRate<TKey>
        {
            Id = model.Id!,
            HourlyRate = model.HourlyRate,
            DailyRate = model.DailyRate,
            MonthlyRate = model.MonthlyRate,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };
    }

    public static CreatedParkingRateDto<TKey> ToCreatedParkingRateDto<TKey>(this ParkingRate<TKey> model) where TKey : IEquatable<TKey>
    {
        return new CreatedParkingRateDto<TKey>
        {
            Id = model.Id,
            HourlyRate = model.HourlyRate,
            DailyRate = model.DailyRate,
            MonthlyRate = model.MonthlyRate,
            ConcurrencyStamp = model.ConcurrencyStamp,
        };
    }

    public static UpdateParkingRateDto<TKey> ToUpdateParkingRateDto<TKey>(this UpdateParkingRateRequest<TKey> request) where TKey : IEquatable<TKey>
    {
        return new UpdateParkingRateDto<TKey>
        {
            Id = request.Id,
            HourlyRate = request.HourlyRate,
            DailyRate = request.DailyRate,
            MonthlyRate = request.MonthlyRate,
            ConcurrencyStamp = request.ConcurrencyStamp,
        };
    }
}