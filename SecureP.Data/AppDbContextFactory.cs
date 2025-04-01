using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SecureP.Data;

public class AppDbContextFactory<TKey> : IDesignTimeDbContextFactory<AppDbContext<TKey>> where TKey : IEquatable<TKey>
{
    public AppDbContext<TKey> CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.json", optional: false) // Use default config
            .AddJsonFile($"appsettings.{environment}.json", optional: true) // Use environment-specific config
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext<TKey>>();
        var connectionString = configuration.GetConnectionString("SqlServer");

        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException("Connection string 'SqlServer' not found.");

        optionsBuilder.UseSqlServer(connectionString);

        return new AppDbContext<TKey>(optionsBuilder.Options);
    }
}

public class AppDbContextFactory : AppDbContextFactory<string>
{
    public AppDbContextFactory() : base()
    {
    }
}