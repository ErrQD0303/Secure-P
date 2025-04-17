using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Secure_P_Backend;
using SecureP.Identity.Models;
using SecureP.Service.Abstraction.Entities;

namespace ApiTests;

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

    protected override string GetCurrentRouteUrl()
    {
        return this.GetCurrentRouteUrl(null);
    }

    protected virtual string GetCurrentRouteUrl(string? id)
    {
        return string.IsNullOrEmpty(id) ? BuildUrl(base.GetCurrentRouteUrl(), CurrentUrl) : BuildUrl(base.GetCurrentRouteUrl(), CurrentUrl, id);
    }

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

    private async Task GetTokenAsync()
    {
        var tokenResponse = await Client.PostAsJsonAsync(BuildUrl(GetCurrentRouteUrl(), TokenUrl), new TokenRequest
        {
            Email = AdminEmail,
            Password = AdminPassword
        });

        tokenResponse.EnsureSuccessStatusCode();

        TokenResponse? tokenResponseObject = await tokenResponse.Content.ReadFromJsonAsync<TokenResponse>();

        ArgumentNullException.ThrowIfNull(tokenResponseObject, nameof(tokenResponseObject));

        Token = tokenResponseObject.AccessToken;
        RefreshToken = tokenResponseObject.RefreshToken;
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(Scheme, Token);
    }

    private async Task RefreshTokenAsync()
    {
        var refreshResponse = await Client.PostAsJsonAsync(BuildUrl(GetCurrentRouteUrl(), RefreshUrl), new RefreshTokenRequest
        {
            RefreshToken = RefreshToken
        });

        refreshResponse.EnsureSuccessStatusCode();

        TokenResponse? tokenResponseObject = await refreshResponse.Content.ReadFromJsonAsync<TokenResponse>();

        ArgumentNullException.ThrowIfNull(tokenResponseObject, nameof(tokenResponseObject));

        Token = tokenResponseObject.AccessToken;
        RefreshToken = tokenResponseObject.RefreshToken;
        Client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(Scheme, Token);
    }
}
