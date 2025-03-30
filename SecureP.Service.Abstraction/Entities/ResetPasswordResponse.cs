using System.Text.Json.Serialization;

namespace SecureP.Service.Abstraction.Entities;

public class ResetPasswordResponse
{
    public int StatusCode { get; set; }
    public string Success { get; set; } = default!;
    public string Message { get; set; } = default!;
    public Dictionary<string, string> Errors { get; set; } = default!;
}