using SecureP.Identity.Models;
using SecureP.Service.Abstraction.Entities;

namespace ApiTests.Fixtures;

public class IdentityFixture : IDisposable
{
    public static RegisterRequest GetRegisterRequest() => new()
    {
        AccountType = "Individual",
        FullName = "Nguyễn Quốc Đạt",
        Email = $"confirm-{Guid.NewGuid():N}@example.com",
        PhoneNumber = "0123456789",
        City = "Hồ Chí Minh",
        Password = "Quocdat@123",
        AddressLine1 = "123 Main St",
        AddressLine2 = "Apt 4B",
        DayOfBirth = new DateTime(1996, 3, 3),
        PostCode = "700000",
        Country = "Vietnam",
        LicensePlates = ["51A-12345", "51B-67890"]
    };

    private bool _disposed = false;

    public void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // Dispose managed resources here

        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~IdentityFixture()
    {
        Dispose(false);
    }
}