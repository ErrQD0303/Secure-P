using System.Text.Json.Serialization;

namespace SecureP.Service.Abstraction.Entities;

public class RegisterRequest
{
    [JsonPropertyName("account_type")]
    public string? AccountType { get; set; }
    [JsonPropertyName("full_name")]
    public string? FullName { get; set; }
    [JsonPropertyName("email")]
    public string? Email { get; set; }
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }
    [JsonPropertyName("city")]
    public string? City { get; set; }
    [JsonPropertyName("password")]
    public string? Password { get; set; }
    [JsonPropertyName("username")]
    public string? Username { get; set; }
    [JsonPropertyName("address_line1")]
    public string? AddressLine1 { get; set; }
    [JsonPropertyName("address_line2")]
    public string? AddressLine2 { get; set; }
    [JsonPropertyName("country")]
    public string? Country { get; set; }
    [JsonPropertyName("day_of_birth")]
    public DateTime DayOfBirth { get; set; }
    [JsonPropertyName("post_code")]
    public string? PostCode { get; set; }
    [JsonPropertyName("license_plates")]
    public string[] LicensePlates { get; set; } = [];
}