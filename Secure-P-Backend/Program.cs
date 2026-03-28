using Secure_P_Backend;
using Secure_P_Backend.Cors.Extensions;
using SecureP.Data.Seed;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(configure =>
    {
        // {info.description}
        configure.DocumentTitle = "Secure-P Backend API";
        // Path to swagger UI and JSON document
        configure.Path = "/swagger";
        // Path to the JSON document that describes the API
        configure.DocumentPath = "/swagger/{documentName}/swagger.json";
        configure.DocExpansion = "list";
    });
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var defaultPW = builder.Configuration["defaultPW"] ?? string.Empty;
    await SeedData<string>.InitializeAsync(services, defaultPW);
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Serve static files from wwwroot folder
app.UseCookiePolicy();

// app.UseExceptionHandler(); // Add global exception handling middleware
app.UseRouting(); // Add routing middleware

app.UseCors();

app.UseAuthentication(); // Add authentication middleware
app.UseAuthorization(); // Add authorization middleware

// app.MapIdentityApi<AppUser<string>>(); // Map identity API routes

app.MapControllers(); // Map controllers with group name "api"

app.Run();
