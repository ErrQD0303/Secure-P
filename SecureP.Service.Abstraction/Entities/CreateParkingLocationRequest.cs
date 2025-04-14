using System.Text.Json.Serialization;
using SecureP.Identity.Models;

namespace SecureP.Service.Abstraction.Entities;

public class CreateParkingLocationRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("address")]
    public string Address { get; set; } = null!;
    [JsonPropertyName("parking_zones")]
    public IEnumerable<CreateParkingLocationParkingZone> ParkingZones { get; set; } = null!;
    [JsonPropertyName("parking_rate_id")]
    public string ParkingRateId { get; set; } = null!;
}