namespace SecureP.Service.Abstraction.Entities;

public class RefreshTokenRequest : TokenRequest
{
    public string RefreshToken { get; set; } = default!;
}