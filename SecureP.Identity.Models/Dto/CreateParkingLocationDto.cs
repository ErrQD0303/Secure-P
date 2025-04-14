using System.Text.Json.Serialization;

namespace SecureP.Identity.Models.Dto;

public class CreateParkingLocationDto<TKey> where TKey : IEquatable<TKey>
{
    [JsonPropertyName("name")]
    public virtual string Name { get; set; } = default!;
    [JsonPropertyName("address")]
    public virtual string Address { get; set; } = default!;
    [JsonPropertyName("parking_zones")]
    public virtual IEnumerable<CreateParkingLocationParkingZoneDto> ParkingZones { get; set; } = default!;
    [JsonPropertyName("parking_rate_id")]
    public virtual TKey? ParkingRateId { get; set; } = default!;
}