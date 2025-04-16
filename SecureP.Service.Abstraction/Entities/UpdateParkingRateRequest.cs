using System.Text.Json.Serialization;

namespace SecureP.Service.Abstraction.Entities;

public class UpdateParkingRateRequest<TKey> where TKey : IEquatable<TKey>
{
    [JsonPropertyName("id")]
    public TKey Id { get; set; } = default!;
    [JsonPropertyName("hourly_rate")]
    public double HourlyRate { get; set; }
    [JsonPropertyName("daily_rate")]
    public double DailyRate { get; set; }
    [JsonPropertyName("monthly_rate")]
    public double MonthlyRate { get; set; }
    [JsonPropertyName("concurrency_stamp")]
    public string ConcurrencyStamp { get; set; } = default!;
}