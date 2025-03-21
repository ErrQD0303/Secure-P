using System.Text.Json.Serialization;

namespace SecureP.Service.Abstraction.Entities;

public class RegisterRequest
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    [JsonPropertyName("password")]
    public required string Password { get; set; }
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }
    [JsonPropertyName("username")]
    public string? Username { get; set; }
    [JsonPropertyName("full_name")]
    public required string FullName { get; set; }
    [JsonPropertyName("address_line1")]
    public required string AddressLine1 { get; set; }
    [JsonPropertyName("address_line2")]
    public string? AddressLine2 { get; set; }
    [JsonPropertyName("city")]
    public required string City { get; set; }
    [JsonPropertyName("country")]
    public required string Country { get; set; }
    [JsonPropertyName("day_of_birth")]
    public required string DayOfBirth { get; set; }
    [JsonPropertyName("post_code")]
    public required string PostCode { get; set; }
    [JsonPropertyName("license_plates")]
    public string[] LicensePlates { get; set; } = [];
}