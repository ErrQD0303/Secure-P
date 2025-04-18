using System.Text.Json.Serialization;

namespace SecureP.Service.Abstraction.Entities;

public class CreateParkingRateRequest
{
    [JsonPropertyName("hourly_rate")]
    public double HourlyRate { get; set; }
    [JsonPropertyName("daily_rate")]
    public double DailyRate { get; set; }
    [JsonPropertyName("monthly_rate")]
    public double MonthlyRate { get; set; }
}