using SecureP.Identity.Models;
using SecureP.Identity.Models.Dto;
using SecureP.Repository.Abstraction.Models;

namespace SecureP.Repository.ParkingLocations.Mappers;

public static class ParkingLocationsRepositoryMappers
{
    public static ParkingLocationValidationModel<TKey> ToParkingLocationValidationModel<TKey>(this CreateParkingLocationDto<TKey> parkingLocation) where TKey : IEquatable<TKey>
    {
        return new ParkingLocationValidationModel<TKey>
        {
            Name = parkingLocation.Name,
            Address = parkingLocation.Address,
            ParkingRateId = parkingLocation.ParkingRateId,
            ParkingZones = [.. parkingLocation.ParkingZones.Select(z => new ParkingLocationParkingZoneValidationModel
            {
                Name = z.Name,
                Capacity = z.Capacity,
                AvailableSpaces = z.AvailableSpaces
            })]
        };
    }

    public static ParkingLocationValidationModel<TKey> ToParkingLocationValidationModel<TKey>(this UpdateParkingLocationDto<TKey> parkingLocation) where TKey : IEquatable<TKey>
    {
        return new ParkingLocationValidationModel<TKey>
        {
            Name = parkingLocation.Name ?? string.Empty,
            Address = parkingLocation.Address ?? string.Empty,
            ParkingRateId = parkingLocation.ParkingRateId,
            ParkingZones = [.. parkingLocation.ParkingZones.Select(z => new ParkingLocationParkingZoneValidationModel
            {
                Name = z.Name,
                Capacity = z.Capacity,
                AvailableSpaces = z.AvailableSpaces
            })]
        };
    }

    public static GetParkingLocationDto<TKey> ToGetParkingLocationDto<TKey>(this ParkingLocation<TKey> parkingLocation) where TKey : IEquatable<TKey>
    {
        var parkingRate = parkingLocation.ParkingLocationRates.FirstOrDefault(x => x.EffectiveFrom <= DateTime.UtcNow && (x.EffectiveTo == null || x.EffectiveTo >= DateTime.UtcNow))?.ParkingRate;
        var parkingLocationDto = new GetParkingLocationDto<TKey>
        {
            Id = parkingLocation.Id,
            Name = parkingLocation.Name,
            Address = parkingLocation.Address,
            ParkingZones = [.. parkingLocation.ParkingZones.Select(z => new GetParkingLocationParkingZoneDto<TKey>
            {
                Id = z.Id,
                Name = z.Name,
                Capacity = z.Capacity,
                AvailableSpaces = z.AvailableSpaces
            })],
            ConcurrencyStamp = parkingLocation.ConcurrencyStamp ?? string.Empty,
            ParkingRate = new GetParkingLocationParkingRateDto<TKey>
            {
                Id = parkingRate!.Id,
                HourlyRate = parkingRate.HourlyRate,
                DailyRate = parkingRate.DailyRate,
                MonthlyRate = parkingRate.MonthlyRate,
            }
        };

        return parkingLocationDto;
    }
}