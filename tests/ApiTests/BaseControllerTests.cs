using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Secure_P_Backend;
using SecureP.Identity.Models;
using SecureP.Service.Abstraction.Entities;

namespace ApiTests;

public class BaseControllerTests(TestApplicationFactory factory) : IClassFixture<TestApplicationFactory>
{
    public HttpClient Client { get; private set; } = factory.CreateClient();
    public string CurrentUrl { get; private set; } = "https://localhost:7081"; // Base URL for the API

    protected static string BuildUrl(params string[] segments) => Path.Combine(segments).Replace('\\', '/');

    protected virtual string GetCurrentRouteUrl() => CurrentUrl;
}
