using System.Text.Json.Serialization;

namespace SecureP.Service.Abstraction.Entities;

public class UserRegisterResponse<TKey> where TKey : IEquatable<TKey>
{
    public int StatusCode { get; set; }
    public string Success { get; set; } = default!;
    public string Message { get; set; } = default!;
    public Dictionary<string, object> Errors { get; set; } = default!;
    public UserRegisterResponseAppUser<TKey> User { get; set; } = default!;

}
public class UserRegisterResponseAppUser<TKey> where TKey : IEquatable<TKey>
{
    [JsonPropertyName("id")]
    public TKey Id { get; set; } = default!;
    [JsonPropertyName("username")]
    public string Username { get; set; } = default!;
    [JsonPropertyName("email")]
    public string Email { get; set; } = default!;
    [JsonPropertyName("emailConfirmed")]
    public bool EmailConfirmed { get; set; }
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
    public ICollection<string> UserLicensePlates
    { get; set; } = default!;
    public string? Avatar { get; set; }
}