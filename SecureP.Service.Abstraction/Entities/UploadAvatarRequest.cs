using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace SecureP.Service.Abstraction.Entities;

public class UploadAvatarRequest
{
    [JsonPropertyName("avatar")]
    public IFormFile Avatar { get; set; } = default!;
}