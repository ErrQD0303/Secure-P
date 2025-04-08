using System.Collections;
using System.Text.Json.Serialization;

namespace SecureP.Identity.Models.Dto;

public class GetAllParkingLocationsDto<TKey> where TKey : IEquatable<TKey>
{
    [JsonPropertyName("items")]
    public List<GetParkingLocationDto<TKey>> Items { get; set; } = [];
    [JsonPropertyName("total_pages")]
    public int TotalPages { get; set; }
}