using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SecureP.Data;
using SecureP.Repository.Abstraction;
using SecureP.Repository.ParkingRates;
using SecureP.Repository.ParkingZones;
using SecureP.Service.Abstraction;
using SecureP.Service.ParkingRateService;
using SecureP.Service.ParkingZoneService;

namespace RepositoryTests.Fixtures;

public class SqlServerTestDbFixture<TKey> : IDisposable where TKey : IEquatable<TKey>
{
    private bool _disposed = false;
    private readonly string _connectionString;

    public AppDbContext<TKey> Context { get; private set; }

    public IParkingRateRepository<TKey> ParkingRateRepository { get; private set; }
    public IParkingRateService<TKey> ParkingRateService { get; private set; }

    public IParkingZoneRepository<TKey> ParkingZoneRepository { get; private set; }
    public IParkingZoneService<TKey> ParkingZoneService { get; private set; }

    public SqlServerTestDbFixture()
    {
        _connectionString = "Server=.;Database=SecurePDb-Test;Integrated Security=true;TrustServerCertificate=True;MultipleActiveResultSets=true;";

        var options = new DbContextOptionsBuilder<AppDbContext<TKey>>()
            .UseSqlServer(_connectionString)
            .Options;

        Context = new AppDbContext<TKey>(options);

        // Ensure the database is created and apply migrations if necessary
        Context.Database.EnsureCreated();

        // Clean up the tables before each test
        Context.ParkingRates.RemoveRange(Context.ParkingRates);
        Context.SaveChanges();

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        var parkingZoneRepositoryLogger = loggerFactory.CreateLogger<ParkingZoneRepository<TKey>>();
        ParkingZoneRepository = new ParkingZoneRepository<TKey>(parkingZoneRepositoryLogger, Context);

        var parkingZoneServiceLogger = loggerFactory.CreateLogger<ParkingZoneService<TKey>>();
        ParkingZoneService = new ParkingZoneService<TKey>(parkingZoneServiceLogger, ParkingZoneRepository);

        var parkingRateRepositoryLogger = loggerFactory.CreateLogger<ParkingRateRepository<TKey>>();
        ParkingRateRepository = new ParkingRateRepository<TKey>(parkingRateRepositoryLogger, Context);

        var parkingRateServiceLogger = loggerFactory.CreateLogger<ParkingRateService<TKey>>();
        ParkingRateService = new ParkingRateService<TKey>(parkingRateServiceLogger, ParkingRateRepository);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources here.
                Context.ParkingRates.RemoveRange(Context.ParkingRates);
                Context.SaveChanges();
                Context.Database.EnsureDeleted();
            }

            // Dispose unmanaged resources here.

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    // For unmanaged resources, if any, implement a finalizer.
    ~SqlServerTestDbFixture()
    {
        Dispose(disposing: false);
    }
}