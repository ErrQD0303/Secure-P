namespace SecureP.Service.Abstraction.Entities;

public class UpdateParkingLocationResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = null!;
    public bool Success { get; set; }
    public Dictionary<string, object> Errors { get; set; } = default!;
}