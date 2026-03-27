using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Secure_P_Backend;
using SecureP.Identity.Models;
using SecureP.Service.Abstraction.Entities;

namespace ApiTests;

public class BaseControllerTests(TestApplicationFactory factory) : IClassFixture<TestApplicationFactory>, IDisposable
{
    private bool _disposed = false;
    protected readonly IServiceScope _scope = factory.Services.CreateScope();

    public TestApplicationFactory Factory => factory;
    public HttpClient Client { get; private set; } = factory.CreateClient();
    public string CurrentUrl { get; private set; } = "https://localhost:7081"; // Base URL for the API

    protected static string BuildUrl(params string[] segments) => Path.Combine(segments).Replace('\\', '/');

    protected virtual string GetCurrentRouteUrl() => CurrentUrl;

    public virtual void DisposeResources()
    {
        Client.Dispose();
        _scope.Dispose();
    }

    public void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            DisposeResources();
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~BaseControllerTests()
    {
        Dispose(false);
    }
}
