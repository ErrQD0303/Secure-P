using System.Text.Json.Serialization;
using SecureP.Identity.Models;

namespace SecureP.Service.Abstraction.Entities;

public class LoginResponse
{
    public int StatusCode { get; set; }
    public string Success { get; set; } = default!;
    public string Message { get; set; } = default!;
    public Dictionary<string, string> Errors { get; set; } = default!;
    public LoginResponseAppUser<string> User { get; set; } = default!;
}

public class LoginResponseAppUser<TKey> where TKey : IEquatable<TKey>
{
    [JsonPropertyName("statusCode")]
    public string Id { get; set; } = default!;
    [JsonPropertyName("username")]
    public string Username { get; set; } = default!;
    [JsonPropertyName("email")]
    public string Email { get; set; } = default!;
    [JsonPropertyName("mobileNumber")]
    public string PhoneNumber { get; set; } = default!;
    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = default!;
    [JsonPropertyName("country")]
    public string Country { get; set; } = default!;
    [JsonPropertyName("dayOfBirth")]
    public DateTime DayOfBirth { get; set; } = default!;
    [JsonPropertyName("city")]
    public string City { get; set; } = default!;
    [JsonPropertyName("addressLine1")]
    public string AddressLine1 { get; set; } = default!;
    [JsonPropertyName("addressLine2")]
    public string? AddressLine2 { get; set; } = default!;
    [JsonPropertyName("postCode")]
    public string PostCode { get; set; } = default!;
    [JsonPropertyName("licensePlateNumber")]
    public ICollection<AppUserLicensePlate<TKey>> UserLicensePlates
    { get; set; } = default!;
}