using Secure_P_Backend.Cors.Extensions;

namespace Secure_P_Backend.Cors.Extensions;

public static class CorsExtensions
{
    public static WebApplication UseCors(this WebApplication app)
    {
        var CORSConfig = app.Services.GetRequiredService<IOptions<CorsConfigures>>().Value;
        foreach (var origin in CORSConfig?.Origins ?? [])
        {
            if (!string.IsNullOrEmpty(origin.Name) && !string.IsNullOrEmpty(origin.Origins))
            {
                app.UseCors(origin.Name);
            }
        }

        return app;
    }
}