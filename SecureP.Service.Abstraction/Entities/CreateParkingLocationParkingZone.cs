using System.Text.Json.Serialization;

namespace SecureP.Service.Abstraction.Entities;

public class CreateParkingLocationParkingZone
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("capacity")]
    public int Capacity { get; set; } = default!;
    [JsonPropertyName("available_spaces")]
    public int AvailableSpaces { get; set; } = default!;
}