using SecureP.Identity.Models;

namespace ApiTests.Fixtures;

public class ParkingRateFixture : IDisposable
{
    public List<ParkingRate<string>>? ParkingRates { get; set; } = default!;

    public ParkingRateFixture()
    {
        ParkingRates = [];
    }
    private bool _disposed = false;

    public void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // Dispose managed resources here
        }

        // Dispose unmanaged resources here

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~ParkingRateFixture()
    {
        Dispose(false);
    }
}