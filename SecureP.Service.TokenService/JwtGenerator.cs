using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace SecureP.Service.TokenService;

public class JwtGenerator
{
    public static string GenerateJWTToken(string issuer, string audience, string? nonce, int expirySeconds, IEnumerable<Claim> claims, JsonWebKey jwk, Dictionary<string, object>? additionalClaims = null)
    {
        var signingCredentials = new SigningCredentials(jwk, SecurityAlgorithms.HmacSha256);

        var jwtHeader = new JwtHeader(signingCredentials);

        additionalClaims ??= [];

        if (!string.IsNullOrEmpty(nonce))
        {
            additionalClaims.Add(JwtRegisteredClaimNames.Nonce, nonce);
        }

        var jwtPayload = new JwtPayload(issuer, audience, claims, additionalClaims, DateTime.UtcNow, DateTime.UtcNow.AddSeconds(expirySeconds), DateTime.UtcNow);

        var token = new JwtSecurityToken(jwtHeader, jwtPayload);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}