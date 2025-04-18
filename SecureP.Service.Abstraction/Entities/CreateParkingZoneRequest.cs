using System.Text.Json.Serialization;

namespace SecureP.Service.Abstraction.Entities;

public class CreateParkingZoneRequest<TKey> where TKey : IEquatable<TKey>
{
    [JsonPropertyName("name")]
    public virtual string Name { get; set; } = default!;
    [JsonPropertyName("parking_location_id")]
    public virtual TKey? ParkingLocationId { get; set; } = default!;
    [JsonPropertyName("capacity")]
    public virtual int Capacity { get; set; } = default!;
    [JsonPropertyName("available_spaces")]
    public virtual int AvailableSpaces { get; set; } = default!;
}