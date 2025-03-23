using System.Text.Json.Serialization;

namespace SecureP.Service.Abstraction.Entities;

public class OTPLoginRequest
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    [JsonPropertyName("otp")]
    public required string OTP { get; set; }
}