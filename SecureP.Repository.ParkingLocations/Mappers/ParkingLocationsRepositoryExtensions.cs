using SecureP.Identity.Models;
using SecureP.Identity.Models.Dto;
using SecureP.Repository.Abstraction.Models;

namespace SecureP.Repository.ParkingLocations.Mappers;

public static class ParkingLocationsRepositoryExtensions
{
    public static ParkingLocationValidationModel ToParkingLocationValidationModel(this CreateParkingLocationDto parkingLocation)
    {
        return new ParkingLocationValidationModel
        {
            Name = parkingLocation.Name,
            Address = parkingLocation.Address,
            Capacity = parkingLocation.Capacity,
            AvailableSpaces = parkingLocation.AvailableSpaces,
            ParkingRate = new ParkingLocationParkingRateValidationModel
            {
                HourlyRate = parkingLocation.ParkingRate?.HourlyRate ?? 0,
                DailyRate = parkingLocation.ParkingRate?.DailyRate ?? 0,
                MonthlyRate = parkingLocation.ParkingRate?.MonthlyRate ?? 0
            }
        };
    }
    public static ParkingLocationValidationModel ToParkingLocationValidationModel(this UpdateParkingLocationDto parkingLocation)
    {
        return new ParkingLocationValidationModel
        {
            Name = parkingLocation.Name ?? string.Empty,
            Address = parkingLocation.Address ?? string.Empty,
            Capacity = parkingLocation.Capacity ?? 0,
            AvailableSpaces = parkingLocation.AvailableSpaces ?? 0,
            ParkingRate = new ParkingLocationParkingRateValidationModel
            {
                HourlyRate = parkingLocation.ParkingRate?.HourlyRate ?? 0,
                DailyRate = parkingLocation.ParkingRate?.DailyRate ?? 0,
                MonthlyRate = parkingLocation.ParkingRate?.MonthlyRate ?? 0
            }
        };
    }

    public static GetParkingLocationDto<TKey> ToGetParkingLocationDto<TKey>(this ParkingLocation<TKey> parkingLocation) where TKey : IEquatable<TKey>
    {
        var parkingLocationDto = new GetParkingLocationDto<TKey>
        {
            Id = parkingLocation.Id,
            Name = parkingLocation.Name,
            Address = parkingLocation.Address,
            Capacity = parkingLocation.Capacity,
            AvailableSpaces = parkingLocation.AvailableSpaces,
            ConcurrencyStamp = parkingLocation.ConcurrencyStamp ?? string.Empty
        };

        if (parkingLocation.ParkingRate != null)
        {
            parkingLocationDto.HourlyRate = parkingLocation.ParkingRate.HourlyRate;
            parkingLocationDto.DailyRate = parkingLocation.ParkingRate.DailyRate;
            parkingLocationDto.MonthlyRate = parkingLocation.ParkingRate.MonthlyRate;
        }

        return parkingLocationDto;
    }
}