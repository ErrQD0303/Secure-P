namespace SecureP.Service.Abstraction.Entities;

public class CreateParkingRateResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = null!;
    public bool Success { get; set; }
    public Dictionary<string, object> Errors { get; set; } = default!;
}