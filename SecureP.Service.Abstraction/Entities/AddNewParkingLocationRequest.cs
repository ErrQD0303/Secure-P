using SecureP.Identity.Models;

namespace SecureP.Service.Abstraction.Entities;

public class AddNewParkingLocationRequest
{
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public int Capacity { get; set; }
    public ParkingRate<string> ParkingRate { get; set; } = default!;
}