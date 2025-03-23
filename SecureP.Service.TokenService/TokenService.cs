using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecureP.Identity.Models;
using SecureP.Repository.Abstraction;
using SecureP.Service.Abstraction;
using SecureP.Service.Abstraction.Entities;
using SecureP.Service.Abstraction.Exceptions;
using SecureP.Shared;
using SecureP.Shared.Configures;

namespace SecureP.Service.TokenService;

public class TokenService<TKey> : ITokenService where TKey : IEquatable<TKey>
{
    private readonly ITokenRepository<TKey> _tokenRepository;
    private readonly ILogger<TokenService<TKey>> _logger;
    private readonly UserManager<AppUser<TKey>> _userManager;
    private readonly IConfiguration _configuration;
    private readonly JwtConfigures _jwtConfigures;

    public TokenService(ITokenRepository<TKey> tokenRepository, ILogger<TokenService<TKey>> logger, UserManager<AppUser<TKey>> userManager, IConfiguration configuration, IOptions<JwtConfigures> jwtConfigures)
    {
        _tokenRepository = tokenRepository;
        _logger = logger;
        _userManager = userManager;
        _configuration = configuration;
        _jwtConfigures = jwtConfigures.Value;
    }

    public async Task<string> GenerateAccessTokenAsync(TokenRequest tokenRequest)
    {
        _logger.LogInformation("Generating Access Token");

        var jwk = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt Key is missing")));

        var jsonWebKey = new JsonWebKey
        {
            Kty = "oct",
            K = Base64UrlEncoder.Encode(jwk.Key)
        };

        AppUser<TKey>? user;

        if (tokenRequest.Email != null)
        {
            user = await _userManager.FindByEmailAsync(tokenRequest.Email);
        }
        else
        {
            user = await _userManager.FindByNameAsync(tokenRequest?.Username?.ToString() ?? string.Empty);
        }

        if (user == null)
        {
            return string.Empty;
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()!),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!)
        };

        var accessToken = JwtGenerator.GenerateJWTToken(_configuration["Jwt:Authority"] ?? throw new InvalidOperationException("Jwt Authority is missing"), _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt Audience is missing"), null, int.Parse(_configuration["Jwt:ExpirySeconds"] ?? throw new InvalidOperationException("Jwt ExpirySeconds is missing")), claims, jsonWebKey);

        await AddUserTokenAsync(tokenRequest!, accessToken, TokenType.AccessToken);

        return accessToken;
    }

    public async Task<string> GenerateRefreshTokenAsync(TokenRequest tokenRequest)
    {
        _logger.LogInformation("Generating Refresh Token");

        var refreshToken = GenerateRandomString();

        await AddUserTokenAsync(tokenRequest, refreshToken, TokenType.RefreshToken);

        return refreshToken;
    }

    private async Task AddUserTokenAsync(TokenRequest tokenRequest, string refreshToken, TokenType tokenType)
    {
        var userLoginProviderInfo = (await _userManager.GetLoginsAsync(new AppUser<TKey> { UserName = tokenRequest?.Username?.ToString() })).FirstOrDefault();

        var user = (tokenRequest?.Email != null ? await _userManager.FindByEmailAsync(tokenRequest.Email) : await _userManager.FindByNameAsync(tokenRequest?.Username?.ToString() ?? string.Empty)) ?? throw new TokenServiceException(string.Format("User {0} not found", tokenRequest?.Username?.ToString() ?? tokenRequest?.Email ?? string.Empty));

        var expiryDate = tokenType switch
        {
            TokenType.AccessToken => DateTime.UtcNow.AddSeconds(_jwtConfigures.ExpirySeconds),
            TokenType.RefreshToken => DateTime.UtcNow.AddDays(_jwtConfigures.RefreshExpirySeconds),
            _ => throw new TokenServiceException("Invalid Token Type")
        };

        await _tokenRepository.AddTokenAsync(refreshToken, user!.Id, tokenType, DateTime.UtcNow.AddDays(7), userLoginProviderInfo != null ? userLoginProviderInfo.LoginProvider : AppConstants.DefaultLoginProvider);
    }

    public async Task<bool> ValidateAccessTokenAsync(string accessToken, string username)
    {
        _logger.LogInformation("Validating Access Token");

        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            return false;
        }

        var loginProviderInfo = (await _userManager.GetLoginsAsync(user)).FirstOrDefault();

        return await _tokenRepository.ValidateTokenAsync(accessToken, user.Id, TokenType.AccessToken, loginProviderInfo != null ? loginProviderInfo.LoginProvider : AppConstants.DefaultLoginProvider);
    }

    public async Task<bool> ValidateRefreshTokenAsync(RefreshTokenRequest request)
    {
        _logger.LogInformation("Validating Refresh Token");

        AppUser<TKey>? user;

        if (request.Email != null)
        {
            user = await _userManager.FindByEmailAsync(request.Email);
        }
        else
        {
            user = await _userManager.FindByNameAsync(request?.Username?.ToString() ?? string.Empty);
        }

        if (user == null)
        {
            return false;
        }

        var userLoginInfo = (await _userManager.GetLoginsAsync(user)).FirstOrDefault();

        return await _tokenRepository.ValidateTokenAsync(request?.RefreshToken!, user.Id, TokenType.RefreshToken, userLoginInfo != null ? userLoginInfo.LoginProvider : AppConstants.DefaultLoginProvider);
    }

    private static readonly Random random = new();

    private static string GenerateRandomString(int length = 32)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        return new string([.. Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()]);
    }

    public async Task<string> GenerateOTPAsync(string email)
    {
        _logger.LogInformation("Generating OTP");

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            throw new TokenServiceException($"User with email {email} not found");
        }

        var otp = new Random().Next(100000, 999999).ToString();

        await _tokenRepository.AddTokenAsync(otp, user.Id, TokenType.OTP, DateTime.Now.AddMinutes(AppConstants.OTPConstant.ExpiryMinute), AppConstants.DefaultLoginProvider);

        return otp;
    }

    public async Task<bool> ValidateOTPAsync(string email, string otp)
    {
        _logger.LogInformation("Validating OTP");

        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return false;
        }

        var userLoginInfo = (await _userManager.GetLoginsAsync(user)).FirstOrDefault();

        return await _tokenRepository.ValidateTokenAsync(otp, user.Id, TokenType.OTP, userLoginInfo != null ? userLoginInfo.LoginProvider : AppConstants.DefaultLoginProvider);
    }
}
