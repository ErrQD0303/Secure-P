using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Secure_P_Backend;
using SecureP.Identity.Models;
using SecureP.Service.Abstraction.Entities;

namespace ApiTests;

/// <summary>
/// Base class for API controller tests, providing common functionality for authentication and token management.
/// </summary>
/// <param name="factory">TestApplicationFactory object</param>
public class ApiControllerTests(TestApplicationFactory factory) : BaseControllerTests(factory)
{
    public new string CurrentUrl { get; private set; } = "api"; // Base URL for the API
    public string TokenUrl { get; private set; } = "Token/token";
    public string Token { get; private set; } = string.Empty;
    public string RefreshUrl { get; private set; } = "Token/refresh";
    public string RefreshToken { get; private set; } = string.Empty;
    public string Scheme { get; private set; } = "Bearer";
    public string AdminEmail { get; private set; } = "Admin@SecureP.com";
    public string AdminPassword { get; private set; } = "Admin@123";

    /// <summary>
    /// Gets the current route URL for the API controller tests. This method can be overridden by derived classes to provide specific route URLs.
    /// </summary>
    /// <returns>the current url</returns>
    protected override string GetCurrentRouteUrl()
    {
        return GetCurrentRouteUrl(null);
    }

    /// <summary>
    /// Gets the current route URL for the API controller tests, optionally including an ID segment.
    /// </summary>
    /// <param name="id">id for the route</param>
    /// <returns></returns>
    protected virtual string GetCurrentRouteUrl(string? id)
    {
        return string.IsNullOrEmpty(id) ? BuildUrl(base.GetCurrentRouteUrl(), CurrentUrl) : BuildUrl(base.GetCurrentRouteUrl(), CurrentUrl, id);
    }

    /// <summary>
    /// Ensures that a valid token is available for authentication.
    /// </summary>
    /// <returns></returns>
    protected async Task EnsureTokenAsync()
    {
        if (string.IsNullOrEmpty(Token))
        {
            if (string.IsNullOrEmpty(RefreshToken))
            {
                await GetTokenAsync();
            }
            else
            {
                await RefreshTokenAsync();
            }
        }
    }

    /// <summary>
    /// Obtains a new token using the admin credentials and sets the authorization header for the HTTP client.
    /// </summary>
    /// <returns></returns>
    private async Task GetTokenAsync()
    {
        var tokenResponse = await Client.PostAsJsonAsync(BuildUrl(GetCurrentRouteUrl(), TokenUrl), new TokenRequest
        {
            Email = AdminEmail,
            Password = AdminPassword
        });

        await SetTokenHeaders(tokenResponse);
    }

    /// <summary>
    /// Refreshes the token using the current refresh token and updates the authorization header for the HTTP client.
    /// </summary>
    /// <returns></returns>
    private async Task RefreshTokenAsync()
    {
        var refreshResponse = await Client.PostAsJsonAsync(BuildUrl(GetCurrentRouteUrl(), RefreshUrl), new RefreshTokenRequest
        {
            RefreshToken = RefreshToken
        });

        await SetTokenHeaders(refreshResponse);
    }

    /// <summary>
    /// Sets the token headers for the HTTP client based on the response from the token or refresh token endpoint.
    /// </summary>
    /// <param name="response">Token Http Response</param>
    /// <returns></returns>
    private async Task SetTokenHeaders(HttpResponseMessage? response)
    {
        ArgumentNullException.ThrowIfNull(response, nameof(response));
        response.EnsureSuccessStatusCode();

        TokenResponseDto? tokenResponseObject = await response.Content.ReadFromJsonAsync<TokenResponseDto>();

        ArgumentNullException.ThrowIfNull(tokenResponseObject, nameof(tokenResponseObject));

        Token = tokenResponseObject.AccessToken;
        RefreshToken = tokenResponseObject.RefreshToken;
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(Scheme, Token);
    }
}
