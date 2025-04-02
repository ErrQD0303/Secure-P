namespace SecureP.Identity.Models.Dto;

public class UpdateParkingLocationDto
{
    public virtual string? Name { get; set; } = default!;
    public virtual string? Address { get; set; } = default!;
    public virtual int? Capacity { get; set; } = default!;
    public virtual int? AvailableSpaces { get; set; } = default!;
    public virtual string? ConcurrencyStamp { get; set; } = default!;
    public virtual UpdateParkingRateDto? ParkingRate { get; set; } = default!;
}