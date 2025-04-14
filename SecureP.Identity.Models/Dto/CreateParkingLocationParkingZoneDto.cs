using System.Text.Json.Serialization;

namespace SecureP.Identity.Models.Dto;

public class CreateParkingLocationParkingZoneDto
{
    [JsonPropertyName("name")]
    public virtual string Name { get; set; } = default!;
    [JsonPropertyName("capacity")]
    public virtual int Capacity { get; set; } = default!;
    [JsonPropertyName("available_spaces")]
    public virtual int AvailableSpaces { get; set; } = default!;
}