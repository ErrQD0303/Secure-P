using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SecureP.Data;

/// <summary>
/// Factory for creating instances of AppDbContext during design time (e.g., for migrations). This factory reads the connection string from the appsettings.json configuration file, allowing Entity Framework Core tools to create the DbContext with the correct database connection settings. By implementing IDesignTimeDbContextFactory, this class ensures that the DbContext can be properly instantiated even when the application's dependency injection container is not available, such as during design-time operations. The generic type parameter TKey allows for flexibility in defining the type of the primary key used in the application's identity models.
/// </summary>
/// <typeparam name="TKey"></typeparam>
public class AppDbContextFactory<TKey> : IDesignTimeDbContextFactory<AppDbContext<TKey>> where TKey : IEquatable<TKey>
{
    public AppDbContext<TKey> CreateDbContext(string[] args)
    {
        // 1. Initialize DB context options builder
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

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

        // 2. Create and return the AppDbContext instance
        return new AppDbContext<TKey>(optionsBuilder.Options);
    }
}

/// <summary>
/// Non-generic version of AppDbContextFactory for convenience when using string as the primary key type. This class inherits from the generic AppDbContextFactory with TKey set to string, allowing for easy instantiation of the DbContext factory without needing to specify the generic type parameter when string is the default primary key type used in the application's identity models. By providing this non-generic version, it simplifies the usage of the factory in scenarios where a string primary key is sufficient, while still maintaining the flexibility of the generic version for other key types if needed.
/// </summary>
public class AppDbContextFactory : AppDbContextFactory<string>
{
    public AppDbContextFactory() : base()
    {
    }
}