using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SecureP.Service.Abstraction.Entities;

public class OTPLoginRequest
{
    [Required]
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    [Required]
    [JsonPropertyName("otp")]
    public required string OTP { get; set; }
}