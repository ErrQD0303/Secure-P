using System.Collections;
using System.Text.Json.Serialization;

namespace SecureP.Identity.Models.Dto;

public class GetAllParkingZoneDto<TKey> where TKey : IEquatable<TKey>
{
    [JsonPropertyName("items")]
    public List<GetParkingZoneDto<TKey>> Items { get; set; } = [];
    [JsonPropertyName("total_pages")]
    public int TotalPages { get; set; }
    [JsonPropertyName("total_items")]
    public int TotalItems { get; set; }
}