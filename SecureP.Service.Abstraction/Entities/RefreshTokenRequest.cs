namespace SecureP.Service.Abstraction.Entities;

public class RefreshTokenRequest : TokenRequest
{
    public required string RefreshToken { get; set; }
}