namespace SecureP.Service.Abstraction.Entities;

public class TokenRequest
{
    public string Username { get; set; } = default!;  // For login
    public string Email { get; set; } = default!;  // For login
    public string Password { get; set; } = default!;  // For login
}