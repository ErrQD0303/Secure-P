using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace SecureP.Service.Abstraction.Entities;

public class UpdatePasswordRequest
{
    [JsonPropertyName("old_password")]
    public required string OldPassword { get; set; }
    [JsonPropertyName("new_password")]
    public required string NewPassword { get; set; }
}