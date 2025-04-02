namespace Secure_P_Backend.CORS.Extensions;

public static class CORSExtensions
{
    public static WebApplication UseCORS(this WebApplication app)
    {
        var CORSConfig = app.Services.GetRequiredService<IOptions<CORSConfigures>>().Value;
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