using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace SecureP.Identity.Models.Dto;

public class UpdateParkingLocationDto<TKey> where TKey : IEquatable<TKey>
{
    public virtual string? Name { get; set; } = default!;
    public virtual string? Address { get; set; } = default!;
    public virtual ICollection<UpdateParkingLocationParkingZoneDto<TKey>> ParkingZones { get; set; } = default!;
    public virtual string? ConcurrencyStamp { get; set; } = default!;
    public virtual TKey ParkingRateId { get; set; } = default!;
}