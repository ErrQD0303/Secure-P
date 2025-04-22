using System.Text.Json.Serialization;

namespace SecureP.Identity.Models.Dto;

public class CreatedAppUserParkingSubscriptionDto<TKey> where TKey : IEquatable<TKey>
{
    [JsonPropertyName("id")]
    public virtual TKey Id { get; set; } = default!;
    [JsonPropertyName("user_id")]
    public virtual TKey UserId { get; set; } = default!;
    [JsonPropertyName("parking_zone_id")]
    public virtual TKey ParkingZoneId { get; set; } = default!;
    [JsonPropertyName("product_type")]
    public virtual ProductType ProductType { get; set; } = default!;
    [JsonPropertyName("start_date")]
    public virtual DateTime StartDate { get; set; } = default!;
    [JsonPropertyName("end_date")]
    public virtual DateTime EndDate { get; set; } = default!;
    [JsonPropertyName("subscription_fee")]
    public virtual double SubscriptionFee { get; set; } = default!;
    [JsonPropertyName("clamping_fee")]
    public virtual double ClampingFee { get; set; } = default!;
    [JsonPropertyName("change_signage_fee")]
    public virtual double ChangeSignageFee { get; set; } = default!;
    [JsonPropertyName("is_paid")]
    public virtual bool IsPaid { get; set; } = default!;
    [JsonPropertyName("license_plate")]
    public virtual string LicensePlate { get; set; } = default!;
    [JsonPropertyName("payment_date")]
    public virtual DateTime? PaymentDate { get; set; } = default!;
    [JsonPropertyName("concurrency_stamp")]
    public virtual string ConcurrencyStamp { get; set; } = default!;
}