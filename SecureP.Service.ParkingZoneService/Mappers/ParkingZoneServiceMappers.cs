using SecureP.Identity.Models;
using SecureP.Identity.Models.Dto;
using SecureP.Repository.Abstraction.Models;

namespace SecureP.Service.ParkingZoneService.Mappers;

public static class ParkingZoneServiceMappers
{
    public static ParkingZoneValidationModel<TKey> ToParkingZoneValidationModel<TKey>(this CreateParkingZoneDto<TKey> createParkingZoneDto) where TKey : IEquatable<TKey>
    {
        return new ParkingZoneValidationModel<TKey>
        {
            Id = createParkingZoneDto.Id,
            Name = createParkingZoneDto.Name,
            Capacity = createParkingZoneDto.Capacity,
            AvailableSpaces = createParkingZoneDto.AvailableSpaces
        };
    }

    public static ParkingZone<TKey> ToParkingZone<TKey>(this CreateParkingZoneDto<TKey> createParkingZoneDto) where TKey : IEquatable<TKey>
    {
        return new ParkingZone<TKey>
        {
            Id = createParkingZoneDto.Id,
            Name = createParkingZoneDto.Name,
            Capacity = createParkingZoneDto.Capacity,
            AvailableSpaces = createParkingZoneDto.AvailableSpaces
        };
    }

    public static CreatedParkingZoneDto<TKey> ToCreatedParkingZoneDto<TKey>(this ParkingZone<TKey> parkingZone) where TKey : IEquatable<TKey>
    {
        return new CreatedParkingZoneDto<TKey>
        {
            Id = parkingZone.Id,
            Name = parkingZone.Name,
            Capacity = parkingZone.Capacity,
            AvailableSpaces = parkingZone.AvailableSpaces
        };
    }

    public static GetParkingZoneDto<TKey> ToGetParkingZoneDto<TKey>(this ParkingZone<TKey> parkingZone) where TKey : IEquatable<TKey>
    {
        return new GetParkingZoneDto<TKey>
        {
            Id = parkingZone.Id,
            Name = parkingZone.Name,
            Capacity = parkingZone.Capacity,
            AvailableSpaces = parkingZone.AvailableSpaces
        };
    }

    public static ParkingZoneValidationModel<TKey> ToParkingZoneValidationModel<TKey>(this UpdateParkingZoneDto<TKey> updateParkingZoneDto) where TKey : IEquatable<TKey>
    {
        return new ParkingZoneValidationModel<TKey>
        {
            Id = updateParkingZoneDto.Id,
            Name = updateParkingZoneDto.Name,
            Capacity = updateParkingZoneDto.Capacity,
            AvailableSpaces = updateParkingZoneDto.AvailableSpaces
        };
    }
}