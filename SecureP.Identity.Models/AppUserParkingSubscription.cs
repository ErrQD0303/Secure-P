using System.ComponentModel.DataAnnotations;

namespace SecureP.Identity.Models;

public class AppUserParkingSubscription<TKey, TUserId, TParkingLocationId, TParkingZoneId>
    where TKey : IEquatable<TKey>
    where TUserId : IEquatable<TUserId>
    where TParkingLocationId : IEquatable<TParkingLocationId>
    where TParkingZoneId : IEquatable<TParkingZoneId>
{
    [Key]
    public virtual TKey Id { get; set; } = default!;
    public virtual TUserId UserId { get; set; } = default!;
    public virtual TParkingLocationId ParkingLocationId { get; set; } = default!;
    public virtual TParkingZoneId ParkingZoneId { get; set; } = default!;
    public virtual ProductType ProductType { get; set; } = default!;
    public virtual DateTime StartDate { get; set; } = default!;
    public virtual DateTime EndDate { get; set; } = default!;
    public virtual double SubscriptionFee { get; set; } = default!;
    public virtual double ClampingFee { get; set; } = default!;
    public virtual double ChangeSignageFee { get; set; } = default!;
    public virtual bool IsPaid { get; set; } = default!;
    public virtual string LicensePlate { get; set; } = default!;
    public virtual DateTime? PaymentDate { get; set; } = default!;

    // Navigation properties
    public virtual AppUser<TUserId> User { get; set; } = default!;
    public virtual ParkingLocation<TParkingLocationId> ParkingLocation { get; set; } = default!;
    public virtual ParkingZone<TParkingZoneId> ParkingZone { get; set; } = default!;
}

public class AppUserParkingSubscription<TKey> : AppUserParkingSubscription<TKey, TKey, TKey, TKey>
    where TKey : IEquatable<TKey>
{
}