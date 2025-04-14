using SecureP.Identity.Models.Dto;
using SecureP.Service.Abstraction.Entities;

namespace SecureP.Service.ParkingLocationService.Mappers
{
    public static class ParkingLocationServiceExtensions
    {
        public static CreateParkingLocationDto<TKey> ToCreateParkingLocationDto<TKey>(this CreateParkingLocationRequest request) where TKey : class, IEquatable<TKey>
        {
            return new CreateParkingLocationDto<TKey>
            {
                Name = request.Name,
                Address = request.Address,
                ParkingZones = [.. request.ParkingZones.Select(zone => new CreateParkingLocationParkingZoneDto
                {
                    Name = zone.Name,
                    Capacity = zone.Capacity,
                    AvailableSpaces = zone.AvailableSpaces
                })],
                ParkingRateId = request.ParkingRateId as TKey ?? default!,
            };
        }

        public static UpdateParkingLocationDto<TKey> ToUpdateParkingLocationDto<TKey>(this UpdateParkingLocationRequest request) where TKey : class, IEquatable<TKey>
        {
            return new UpdateParkingLocationDto<TKey>
            {
                Name = request.Name,
                Address = request.Address,
                ParkingZones = [.. request.ParkingZones.Select(zone => new UpdateParkingLocationParkingZoneDto<TKey>
                {
                    Id = zone.Id as TKey ?? default!,
                    Name = zone.Name,
                    Capacity = zone.Capacity,
                    AvailableSpaces = zone.AvailableSpaces
                })],
                ParkingRateId = request.ParkingRateId as TKey ?? default!,
                ConcurrencyStamp = request.ConcurrencyStamp
            };
        }
    }
}