using System.Text.Json.Serialization;

namespace SecureP.Identity.Models.Dto;

public class GetParkingLocationParkingZoneDto<TKey> where TKey : IEquatable<TKey>
{
    [JsonPropertyName("id")]
    public virtual TKey Id { get; set; } = default!;
    [JsonPropertyName("name")]
    public virtual string Name { get; set; } = default!;
    [JsonPropertyName("capacity")]
    public virtual int Capacity { get; set; } = default!;
    [JsonPropertyName("available_spaces")]
    public virtual int AvailableSpaces { get; set; } = default!;
}