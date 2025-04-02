using System.Text.Json.Serialization;

namespace SecureP.Identity.Models.Dto;

public class GetParkingLocationDto<TKey> where TKey : IEquatable<TKey>
{
    [JsonPropertyName("id")]
    public virtual TKey Id { get; set; } = default!;
    [JsonPropertyName("name")]
    public virtual string Name { get; set; } = default!;
    [JsonPropertyName("address")]
    public virtual string Address { get; set; } = default!;
    [JsonPropertyName("capacity")]
    public virtual int Capacity { get; set; } = default!;
    [JsonPropertyName("available_spaces")]
    public virtual int AvailableSpaces { get; set; } = default!;
    [JsonPropertyName("hourly_rate")]
    public virtual double HourlyRate { get; set; } = default!;
    [JsonPropertyName("daily_rate")]
    public virtual double DailyRate { get; set; } = default!;
    [JsonPropertyName("monthly_rate")]
    public virtual double MonthlyRate { get; set; } = default!;
    [JsonPropertyName("concurrency_stamp")]
    // [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public virtual string ConcurrencyStamp { get; set; } = default!;
}