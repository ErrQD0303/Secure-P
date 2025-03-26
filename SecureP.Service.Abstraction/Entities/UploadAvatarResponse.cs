using System.Text.Json.Serialization;

namespace SecureP.Service.Abstraction.Entities;

public class UploadAvatarResponse<TKey> where TKey : IEquatable<TKey>
{
    public int StatusCode { get; set; }
    public string Success { get; set; } = default!;
    public string Message { get; set; } = default!;
    public string? AvatarUrl { get; set; }
    public Dictionary<string, string> Errors { get; set; } = default!;

}