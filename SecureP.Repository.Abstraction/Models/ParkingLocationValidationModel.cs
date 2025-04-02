namespace SecureP.Repository.Abstraction.Models;

public class ParkingLocationValidationModel
{
    public string Name { get; set; } = default!;
    public string Address { get; set; } = default!;
    public int Capacity { get; set; } = default!;
    public int AvailableSpaces { get; set; } = default!;
    public ParkingLocationParkingRateValidationModel ParkingRate { get; set; } = default!;
}