using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace SecureP.Service.Abstraction.Entities;

public class ResetPasswordRequest
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    [JsonPropertyName("password")]
    public required string Password { get; set; }
    [JsonPropertyName("confirm_password")]
    public required string ConfirmPassword { get; set; }
    [JsonPropertyName("token")]
    public required string Token { get; set; }
}