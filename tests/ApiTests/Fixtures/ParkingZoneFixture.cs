using SecureP.Identity.Models;

namespace ApiTests.Fixtures;

public class ParkingZoneFixture : IDisposable
{
    public ParkingRate<string>? ParkingRate { get; set; } = default!;
    public ParkingLocation<string>? ParkingLocation { get; set; } = default!;
    public List<ParkingZone<string>>? ParkingZones { get; set; } = default!;

    public ParkingZoneFixture()
    {
        ParkingZones = [];
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

    ~ParkingZoneFixture()
    {
        Dispose(false);
    }
}