using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Secure_P_Backend;
using SecureP.Data;
using Microsoft.Extensions.DependencyInjection;

namespace ApiTests;

public class TestApplicationFactory : WebApplicationFactory<ProgramMarker>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureServices(services =>
        {
            // Add any test-specific services or configurations herevar sp = services.BuildServiceProvider();
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext<string>>();
            db.Database.EnsureDeleted(); // fresh test DB every time
            db.Database.EnsureCreated();
        });

        builder.ConfigureAppConfiguration((context, config) =>
        {
            // var configuration = config.Build();
            // var connectionString = configuration.GetConnectionString("SqlServer");
            // Console.WriteLine($"Connection String: {connectionString}");
            // System.Console.WriteLine($"Environment: {context.HostingEnvironment.EnvironmentName}");
        });
    }
}