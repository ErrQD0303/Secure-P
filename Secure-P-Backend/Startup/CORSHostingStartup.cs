[assembly: HostingStartup(typeof(Secure_P_Backend.Startup.CORSHostingStartup))]
namespace Secure_P_Backend.Startup;

/// <summary>
/// A hosting startup class that configures Cross-Origin Resource Sharing (CORS) services for the Secure-P Backend application.
/// </summary>
public class CORSHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            // Bind CORS configuration from appsettings.json
            services.Configure<CorsConfigures>(config =>
            {
                config.Origins = context.Configuration.GetSection("CORS").Get<List<CorsConfigures.CorsOrigin>>();
            });

            var corsConfig = new CorsConfigures
            {
                Origins = context.Configuration.GetSection("CORS").Get<List<CorsConfigures.CorsOrigin>>()
            };

            services.AddCors(options =>
            {
                if (corsConfig.Origins != null)
                {
                    foreach (var origin in corsConfig.Origins)
                    {
                        if (!string.IsNullOrEmpty(origin.Name) && !string.IsNullOrEmpty(origin.Origins))
                        {
                            options.AddPolicy(origin.Name, policyBuilder =>
                            {
                                policyBuilder.WithOrigins(origin.Origins.Split(';'))
                                    .AllowCredentials()
                                    .AllowAnyMethod()
                                    .AllowAnyHeader();
                            });
                        }
                    }
                }
            });
        });
    }
}