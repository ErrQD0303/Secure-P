namespace SecureP.Service.Abstraction.Entities;

public class DeleteParkingLocationResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = null!;
    public bool Success { get; set; }
}