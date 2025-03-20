namespace SecureP.Service.Abstraction.Entities;

public class RegisterRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Username { get; set; }
}