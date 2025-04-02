using System.Collections;
using System.Text.Json.Serialization;

namespace SecureP.Identity.Models.Dto;

public class GetAllParkingLocationsDto<TKey> : IEnumerable<GetParkingLocationDto<TKey>>
    where TKey : IEquatable<TKey>
{
    [JsonPropertyName("items")]
    public List<GetParkingLocationDto<TKey>> Items { get; set; } = [];

    public IEnumerator<GetParkingLocationDto<TKey>> GetEnumerator()
    {
        foreach (var item in Items)
        {
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}