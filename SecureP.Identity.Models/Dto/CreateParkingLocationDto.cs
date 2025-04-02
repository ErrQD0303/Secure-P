namespace SecureP.Identity.Models.Dto;

public class CreateParkingLocationDto
{
    public virtual string Name { get; set; } = default!;
    public virtual string Address { get; set; } = default!;
    public virtual int Capacity { get; set; } = default!;
    public virtual int AvailableSpaces { get; set; } = default!;
    public virtual CreateParkingRateDto? ParkingRate { get; set; } = default!;
}