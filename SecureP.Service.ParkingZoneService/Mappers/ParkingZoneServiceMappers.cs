using SecureP.Identity.Models;
using SecureP.Identity.Models.Dto;
using SecureP.Repository.Abstraction.Models;
using SecureP.Service.Abstraction.Entities;

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
            AvailableSpaces = createParkingZoneDto.AvailableSpaces,
            ParkingLocationId = createParkingZoneDto.ParkingLocationId
        };
    }

    public static ParkingZone<TKey> ToParkingZone<TKey>(this CreateParkingZoneDto<TKey> createParkingZoneDto) where TKey : IEquatable<TKey>
    {
        return new ParkingZone<TKey>
        {
            Id = createParkingZoneDto.Id,
            Name = createParkingZoneDto.Name,
            Capacity = createParkingZoneDto.Capacity,
            AvailableSpaces = createParkingZoneDto.AvailableSpaces,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            ParkingLocationId = createParkingZoneDto.ParkingLocationId
        };
    }

    public static CreatedParkingZoneDto<TKey> ToCreatedParkingZoneDto<TKey>(this ParkingZone<TKey> parkingZone) where TKey : IEquatable<TKey>
    {
        return new CreatedParkingZoneDto<TKey>
        {
            Id = parkingZone.Id,
            Name = parkingZone.Name,
            Capacity = parkingZone.Capacity,
            AvailableSpaces = parkingZone.AvailableSpaces,
            ConcurrencyStamp = parkingZone.ConcurrencyStamp ?? string.Empty,
            ParkingLocationId = parkingZone.ParkingLocationId
        };
    }

    public static GetParkingZoneDto<TKey> ToGetParkingZoneDto<TKey>(this ParkingZone<TKey> parkingZone) where TKey : IEquatable<TKey>
    {
        return new GetParkingZoneDto<TKey>
        {
            Id = parkingZone.Id,
            Name = parkingZone.Name,
            Capacity = parkingZone.Capacity,
            AvailableSpaces = parkingZone.AvailableSpaces,
            ParkingLocationId = parkingZone.ParkingLocationId,
            ConcurrencyStamp = parkingZone.ConcurrencyStamp ?? string.Empty,
        };
    }

    public static ParkingZoneValidationModel<TKey> ToParkingZoneValidationModel<TKey>(this UpdateParkingZoneDto<TKey> updateParkingZoneDto) where TKey : IEquatable<TKey>
    {
        return new ParkingZoneValidationModel<TKey>
        {
            Id = updateParkingZoneDto.Id,
            Name = updateParkingZoneDto.Name,
            Capacity = updateParkingZoneDto.Capacity,
            AvailableSpaces = updateParkingZoneDto.AvailableSpaces,
            ParkingLocationId = updateParkingZoneDto.ParkingLocationId
        };
    }

    public static CreateParkingZoneDto<TKey> ToCreateParkingZoneDto<TKey>(this CreateParkingZoneRequest<TKey> request) where TKey : IEquatable<TKey>
    {
        return new CreateParkingZoneDto<TKey>
        {
            Id = typeof(TKey) switch
            {
                Type t when t == typeof(Guid) => (TKey)(object)Guid.NewGuid()!,
                Type t when t == typeof(string) => (TKey)(object)Guid.NewGuid().ToString()!,
                _ => throw new NotSupportedException($"Type {typeof(TKey)} is not supported.")
            },
            Name = request.Name,
            Capacity = request.Capacity,
            AvailableSpaces = request.AvailableSpaces,
            ParkingLocationId = request.ParkingLocationId,
        };
    }

    public static UpdateParkingZoneDto<TKey> ToUpdateParkingZoneDto<TKey>(this UpdateParkingZoneRequest<TKey> request) where TKey : IEquatable<TKey>
    {
        return new UpdateParkingZoneDto<TKey>
        {
            Id = request.Id,
            Name = request.Name,
            Capacity = request.Capacity,
            AvailableSpaces = request.AvailableSpaces,
            ParkingLocationId = request.ParkingLocationId is TKey key ? key : default!,
            ConcurrencyStamp = request.ConcurrencyStamp
        };
    }
}