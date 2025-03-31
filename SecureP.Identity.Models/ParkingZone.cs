namespace SecureP.Identity.Models;

public class ParkingZone<TKey, TPLKey> where TKey : IEquatable<TKey> where TPLKey : IEquatable<TPLKey>
{
    public virtual TKey Id { get; set; } = default!;
    public virtual string Name { get; set; } = default!;
    public virtual TPLKey? ParkingLocationId { get; set; } = default!;
    public virtual int Capacity { get; set; } = default!;
    public virtual int AvailableSpaces { get; set; } = default!;

    // Navigation properties
    public virtual ParkingLocation<TPLKey>? ParkingLocation { get; set; } = default!;
    public virtual ICollection<AppUserParkingSubscription<TKey>> UserParkingSubscriptions { get; set; } = default!;
}

public class ParkingZone<TKey> : ParkingZone<TKey, TKey> where TKey : IEquatable<TKey>
{
}