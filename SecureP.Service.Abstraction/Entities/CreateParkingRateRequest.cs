using System.Text.Json.Serialization;

namespace SecureP.Service.Abstraction.Entities;

public class CreateParkingRateRequest
{
    [JsonPropertyName("HourlyRate")]
    public double HourlyRate { get; set; }
    [JsonPropertyName("DailyRate")]
    public double DailyRate { get; set; }
    [JsonPropertyName("MonthlyRate")]
    public double MonthlyRate { get; set; }
}