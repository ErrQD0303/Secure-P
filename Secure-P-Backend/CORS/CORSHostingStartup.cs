[assembly: HostingStartup(typeof(Secure_P_Backend.CORS.CORSHostingStartup))]
namespace Secure_P_Backend.CORS;

public class CORSHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            // Bind CORS configuration from appsettings.json
            services.Configure<CORSConfigures>(config =>
            {
                config.Origins = context.Configuration.GetSection("CORS").Get<List<CORSConfigures.CORSOrigin>>();
            });

            var corsConfig = new CORSConfigures
            {
                Origins = context.Configuration.GetSection("CORS").Get<List<CORSConfigures.CORSOrigin>>()
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