using System.ComponentModel.DataAnnotations;

namespace SecureP.Identity.Models;

public class ParkingLocationRate<TKey> where TKey : IEquatable<TKey>
{
    // Primary key
    [Key]
    public virtual TKey Id { get; set; } = default!;
    public virtual TKey ParkingLocationId { get; set; } = default!;
    public virtual TKey ParkingRateId { get; set; } = default!;
    public virtual DateTime EffectiveFrom { get; set; }
    public virtual DateTime? EffectiveTo { get; set; }

    // Navigation properties
    public virtual ParkingLocation<TKey> ParkingLocation { get; set; } = default!;
    public virtual ParkingRate<TKey> ParkingRate { get; set; } = default!;
    public virtual ICollection<AppUserParkingSubscription<TKey>> UserParkingSubscriptions { get; set; } = default!;
}