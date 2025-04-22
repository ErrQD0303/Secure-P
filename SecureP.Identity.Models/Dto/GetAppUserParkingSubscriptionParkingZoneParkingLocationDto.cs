using System.Text.Json.Serialization;

namespace SecureP.Identity.Models.Dto;

public class GetAppUserParkingSubscriptionParkingZoneParkingLocationDto<TKey> where TKey : IEquatable<TKey>
{
    [JsonPropertyName("id")]
    public virtual TKey Id { get; set; } = default!;
    [JsonPropertyName("name")]
    public virtual string Name { get; set; } = default!;
    [JsonPropertyName("address")]
    public virtual string Address { get; set; } = default!;
    [JsonPropertyName("parking_rate")]
    public virtual GetAppUserParkingSubscriptionParkingZoneParkingLocationDtoParkingRateDto<TKey> ParkingRate { get; set; } = default!;
    [JsonPropertyName("concurrency_stamp")]
    public virtual string ConcurrencyStamp { get; set; } = default!;
}
