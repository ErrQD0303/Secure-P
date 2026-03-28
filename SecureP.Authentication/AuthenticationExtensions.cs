using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SecureP.Shared;
using SecureP.Shared.Configures;

namespace SecureP.Authentication;

/// <summary>
/// Extension methods for configuring authentication services in the application.
/// </summary>
public static class AuthenticationExtensions
{
    /// <summary>
    /// Adds JWT authentication services to the application's dependency injection container.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddAppAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtConfigures>(configuration.GetSection(AppConstants.JwtConfiguresSection));
        var jwtConfigures = configuration.GetSection(AppConstants.JwtConfiguresSection).Get<JwtConfigures>();

        services.AddAuthentication(
            options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
        )
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtConfigures?.Authority ?? throw new InvalidOperationException("Authority is missing"),

                    ValidateAudience = true,
                    ValidAudience = jwtConfigures?.Audience ?? throw new InvalidOperationException("Audience is missing"),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfigures?.Key ?? throw new InvalidOperationException("Key is missing"))),

                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                };
            });

        return services;
    }
}