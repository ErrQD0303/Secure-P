using System.Text.Json.Serialization;

namespace SecureP.Service.Abstraction.Entities;

public class ForgotPasswordRequest
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    [JsonPropertyName("redirectUrl")]
    public required string RedirectUrl { get; set; }
}