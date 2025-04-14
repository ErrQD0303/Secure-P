using System.ComponentModel.DataAnnotations;

namespace SecureP.Identity.Models;

public class ParkingLocation<TKey> where TKey : IEquatable<TKey>
{
    [Key]
    public virtual TKey Id { get; set; } = default!;
    public virtual string Name { get; set; } = default!;
    public virtual string Address { get; set; } = default!;

    // Concurrency token
    public virtual string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

    // Navigation properties
    public virtual ICollection<ParkingLocationRate<TKey>> ParkingLocationRates { get; set; } = default!;
    public virtual ICollection<ParkingZone<TKey>> ParkingZones { get; set; } = default!;
}