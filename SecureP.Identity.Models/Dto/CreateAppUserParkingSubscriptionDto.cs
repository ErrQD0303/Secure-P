namespace SecureP.Identity.Models.Dto;

public class CreateAppUserParkingSubscriptionDto<TKey> where TKey : IEquatable<TKey>
{
    public virtual TKey UserId { get; set; } = default!;
    public virtual TKey ParkingZoneId { get; set; } = default!;
    public virtual ProductType ProductType { get; set; } = default!;
    public virtual DateTime StartDate { get; set; } = default!;
    public virtual DateTime EndDate { get; set; } = default!;
    public virtual double SubscriptionFee { get; set; } = default!;
    public virtual double ClampingFee { get; set; } = default!;
    public virtual double ChangeSignageFee { get; set; } = default!;
    public virtual bool IsPaid { get; set; } = default!;
    public virtual string LicensePlate { get; set; } = default!;
    public virtual DateTime? PaymentDate { get; set; } = default!;
}