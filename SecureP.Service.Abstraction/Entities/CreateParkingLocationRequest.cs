using System.Text.Json.Serialization;
using SecureP.Identity.Models;

namespace SecureP.Service.Abstraction.Entities;

public class CreateParkingLocationRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("address")]
    public string Address { get; set; } = null!;
    [JsonPropertyName("capacity")]
    public int Capacity { get; set; }
    [JsonPropertyName("available_spaces")]
    public int AvailableSpaces { get; set; }
    [JsonPropertyName("hourly_rate")]
    public double HourlyRate { get; set; }
    [JsonPropertyName("daily_rate")]
    public double DailyRate { get; set; }
    [JsonPropertyName("monthly_rate")]
    public double MonthlyRate { get; set; }

}