namespace SecureP.Service.Abstraction.Entities;

public class AddNewParkingLocationResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = null!;
    public bool Success { get; set; }
    public Dictionary<string, string> Errors { get; set; } = default!;
}