using System.Text.Json.Serialization;

namespace SecureP.Service.Abstraction.Entities;

public class RefreshTokenRequest
{
    [JsonPropertyName("refresh_token")]
    public required string RefreshToken { get; set; }
}