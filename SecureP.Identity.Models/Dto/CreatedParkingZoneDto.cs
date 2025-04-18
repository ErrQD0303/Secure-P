using System.Text.Json.Serialization;

namespace SecureP.Identity.Models.Dto;

public class CreatedParkingZoneDto<TKey> where TKey : IEquatable<TKey>
{
    [JsonPropertyName("id")]
    public virtual TKey Id { get; set; } = default!;
    [JsonPropertyName("name")]
    public virtual string Name { get; set; } = default!;
    [JsonPropertyName("capacity")]
    public virtual int Capacity { get; set; } = default!;
    [JsonPropertyName("available_spaces")]
    public virtual int AvailableSpaces { get; set; } = default!;
    [JsonPropertyName("parking_location_id")]
    public virtual TKey? ParkingLocationId { get; set; } = default!;
    [JsonPropertyName("concurrency_stamp")]
    public virtual string ConcurrencyStamp { get; set; } = default!;
}