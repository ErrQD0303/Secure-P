namespace SecureP.Shared.Configures;

public class JwtConfigures
{
    public string? Audience { get; set; }
    public string? Authority { get; set; }
    public string? Key { get; set; }
    public int ExpirySeconds { get; set; }
    public int RefreshExpirySeconds { get; set; }
}