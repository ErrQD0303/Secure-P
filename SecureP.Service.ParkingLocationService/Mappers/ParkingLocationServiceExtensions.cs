using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SecureP.Identity.Models.Dto;
using SecureP.Service.Abstraction.Entities;

namespace SecureP.Service.ParkingLocationService.Mappers
{
    public static class ParkingLocationServiceExtensions
    {
        public static CreateParkingLocationDto ToCreateParkingLocationDto(this CreateParkingLocationRequest request)
        {
            return new CreateParkingLocationDto
            {
                Name = request.Name,
                Address = request.Address,
                Capacity = request.Capacity,
                AvailableSpaces = request.AvailableSpaces,
                ParkingRate = new CreateParkingRateDto
                {
                    HourlyRate = request.HourlyRate,
                    DailyRate = request.DailyRate,
                    MonthlyRate = request.MonthlyRate
                }
            };
        }

        public static UpdateParkingLocationDto ToUpdateParkingLocationDto(this UpdateParkingLocationRequest request)
        {
            return new UpdateParkingLocationDto
            {
                Name = request.Name,
                Address = request.Address,
                Capacity = request.Capacity,
                AvailableSpaces = request.AvailableSpaces,
                ParkingRate = new UpdateParkingRateDto
                {
                    HourlyRate = request.HourlyRate,
                    DailyRate = request.DailyRate,
                    MonthlyRate = request.MonthlyRate
                },
                ConcurrencyStamp = request.ConcurrencyStamp
            };
        }
    }
}