using System.Text.Json.Serialization;

namespace SecureP.Identity.Models.Dto;

public class GetParkingLocationParkingRateDto<TKey> where TKey : IEquatable<TKey>
{
    [JsonPropertyName("id")]
    public virtual TKey Id { get; set; } = default!;
    [JsonPropertyName("hourly_rate")]
    public virtual double HourlyRate { get; set; }
    [JsonPropertyName("daily_rate")]
    public virtual double DailyRate { get; set; }
    [JsonPropertyName("monthly_rate")]
    public virtual double MonthlyRate { get; set; }
}