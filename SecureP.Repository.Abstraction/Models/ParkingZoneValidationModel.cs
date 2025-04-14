namespace SecureP.Repository.Abstraction.Models;

public class ParkingZoneValidationModel
{
    public string Name { get; set; } = default!;
    public int Capacity { get; set; } = default!;
    public int AvailableSpaces { get; set; } = default!;
}