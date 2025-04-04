[assembly: HostingStartup(typeof(Secure_P_Backend.Startup.SwaggerHostingStartup))]
namespace Secure_P_Backend.Startup;

public class SwaggerHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.AddEndpointsApiExplorer();

            services.AddOpenApiDocument(options =>
            {
                options.Title = "Secure-P Backend";
                options.Version = "v1";
                options.Description = "Secure-P Backend API";
                options.DocumentName = "v1";

                options.AddSecurity("JWT", new NSwag.OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = Microsoft.Net.Http.Headers.HeaderNames.Authorization,
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}.",
                    BearerFormat = "JWT",
                    Scheme = "bearer",
                });

                options.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });
        });
    }
}