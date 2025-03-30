using System.Text.Json.Serialization;

namespace SecureP.Service.Abstraction.Entities;

public class UpdatePasswordResponse
{
    public int StatusCode { get; set; }
    public string Success { get; set; } = default!;
    public string Message { get; set; } = default!;
    public Dictionary<string, object> Errors { get; set; } = default!;

}