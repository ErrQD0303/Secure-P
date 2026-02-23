[assembly: HostingStartup(typeof(Secure_P_Backend.Startup.ExceptionHandlerHostingStartup))]
namespace Secure_P_Backend.Startup;

public class ExceptionHandlerHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            // Register exception handlers
            services.AddExceptionHandler<UserRegisterExceptionHandler>();

            // Register ProblemDetails for standardized error responses
            services.AddProblemDetails();
        });
    }
}