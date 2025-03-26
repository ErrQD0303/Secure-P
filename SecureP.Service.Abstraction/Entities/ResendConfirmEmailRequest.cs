using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace SecureP.Service.Abstraction.Entities;

public class ResendConfirmEmailRequest
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }
}