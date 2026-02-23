namespace SecureP.Shared.Configures;

/// <summary>
/// Configuration class for JWT (JSON Web Token) authentication settings.
/// </summary>
public class JwtConfigures
{
    /// <summary>
    /// Gets or sets the audience claim for the JWT token.
    /// Represents the intended recipient of the token.
    /// </summary>
    public string? Audience { get; set; }

    /// <summary>
    /// Gets or sets the authority that issues the JWT token.
    /// Typically represents the security token service or identity provider.
    /// </summary>
    public string? Authority { get; set; }

    /// <summary>
    /// Gets or sets the secret key used to sign and validate JWT tokens.
    /// Must be kept confidential and have sufficient length for security.
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Gets or sets the expiration time in seconds for access tokens.
    /// Defines how long the token remains valid before requiring re-authentication.
    /// </summary>
    public int ExpirySeconds { get; set; }

    /// <summary>
    /// Gets or sets the expiration time in seconds for refresh tokens.
    /// Defines how long a refresh token can be used to obtain a new access token.
    /// </summary>
    public int RefreshExpirySeconds { get; set; }
}