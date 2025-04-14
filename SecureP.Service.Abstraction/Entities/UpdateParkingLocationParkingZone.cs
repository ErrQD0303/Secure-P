using System.Text.Json.Serialization;

namespace SecureP.Service.Abstraction.Entities;

public class UpdateParkingLocationParkingZone
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;
    [JsonPropertyName("capacity")]
    public int Capacity { get; set; } = default!;
    [JsonPropertyName("available_spaces")]
    public int AvailableSpaces { get; set; } = default!;
}