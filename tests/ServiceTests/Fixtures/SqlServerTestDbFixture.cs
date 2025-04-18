using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SecureP.Data;
using SecureP.Repository.Abstraction;
using SecureP.Repository.ParkingLocations;
using SecureP.Repository.ParkingRates;
using SecureP.Repository.ParkingZones;
using SecureP.Service.Abstraction;
using SecureP.Service.ParkingLocationService;
using SecureP.Service.ParkingRateService;
using SecureP.Service.ParkingZoneService;

namespace ServiceTests.Fixtures;

public class SqlServerTestDbFixture<TKey> : IDisposable where TKey : class, IEquatable<TKey>
{
    private bool _disposed = false;
    private readonly string _connectionString;

    public AppDbContext<TKey> Context { get; private set; }

    public IParkingRateRepository<TKey> ParkingRateRepository { get; private set; }
    public IParkingRateService<TKey> ParkingRateService { get; private set; }
    public IParkingLocationRepository<TKey> ParkingLocationRepository { get; private set; }

    public IParkingZoneRepository<TKey> ParkingZoneRepository { get; private set; }
    public IParkingZoneService<TKey> ParkingZoneService { get; private set; }
    public IParkingLocationService<TKey> ParkingLocationService { get; private set; }

    public SqlServerTestDbFixture()
    {
        _connectionString = "Server=.;Database=SecurePDb-Test;Integrated Security=true;TrustServerCertificate=True;MultipleActiveResultSets=true;";

        var options = new DbContextOptionsBuilder<AppDbContext<TKey>>()
            .UseSqlServer(_connectionString)
            .Options;

        Context = new AppDbContext<TKey>(options);

        // Ensure the database is created and apply migrations if necessary
        // Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();

        // Clean up the tables before each test
        Context.ParkingRates.RemoveRange(Context.ParkingRates);
        Context.ParkingZones.RemoveRange(Context.ParkingZones);
        Context.ParkingLocations.RemoveRange(Context.ParkingLocations);

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        var parkingLocationRepositoryLogger = loggerFactory.CreateLogger<ParkingLocationRepository<TKey>>();
        ParkingLocationRepository = new ParkingLocationRepository<TKey>(parkingLocationRepositoryLogger, Context);

        var parkingLocationServiceLogger = loggerFactory.CreateLogger<ParkingLocationService<TKey>>();
        ParkingLocationService = new ParkingLocationService<TKey>(parkingLocationServiceLogger, ParkingLocationRepository);

        var parkingZoneRepositoryLogger = loggerFactory.CreateLogger<ParkingZoneRepository<TKey>>();
        ParkingZoneRepository = new ParkingZoneRepository<TKey>(parkingZoneRepositoryLogger, Context);

        var parkingZoneServiceLogger = loggerFactory.CreateLogger<ParkingZoneService<TKey>>();
        ParkingZoneService = new ParkingZoneService<TKey>(parkingZoneServiceLogger, ParkingZoneRepository, ParkingLocationRepository);

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