[assembly: HostingStartup(typeof(Secure_P_Backend.Startup.SwaggerHostingStartup))]
namespace Secure_P_Backend.Startup;

/// <summary>
/// A hosting startup class that configures Swagger API services for the Secure-P Backend application.
/// </summary>
public class SwaggerHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.AddEndpointsApiExplorer();

            services.AddOpenApiDocument(options =>
            {
                // {info.title}
                options.Title = "Secure-P Backend";
                // {info.version}
                options.Version = "v1";
                // {info.description}
                options.Description = "Secure-P Backend API";
                // document name used in the URL to access the JSON document
                options.DocumentName = "v1";

                // Add JWT authentication to Swagger UI
                options.AddSecurity("JWT", new NSwag.OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = Microsoft.Net.Http.Headers.HeaderNames.Authorization,
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}.",
                    BearerFormat = "JWT",
                    Scheme = "bearer",
                });

                // Add the security requirement to include the JWT token in the Swagger UI
                options.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });
        });
    }
}