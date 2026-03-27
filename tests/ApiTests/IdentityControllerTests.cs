using System.Net.Http.Json;
using System.Text;
using ApiTests.Fixtures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using SecureP.Identity.Models;
using SecureP.Service.Abstraction.Entities;
using SecureP.Shared;

namespace ApiTests;

[Collection(nameof(IdentityCollection))]
public class IdentityControllerTests : ApiControllerTests
{
    public new string CurrentUrl { get; private set; } = "Identity"; // Base URL for the Identity API
    public AppUser<string>? TestUser { get; private set; }
    public UserManager<AppUser<string>> UserManager { get; private set; }
    public IdentityFixture IdentityFixture { get; private set; }

    public IdentityControllerTests(TestApplicationFactory factory, IdentityFixture fixture) : base(factory)
    {
        IdentityFixture = fixture;
        UserManager = _scope.ServiceProvider.GetRequiredService<UserManager<AppUser<string>>>();
    }

    public override void DisposeResources()
    {
        base.DisposeResources();
        UserManager.Dispose();
    }

    private async Task<HttpResponseMessage> GetRegisteredUserResponseAsync(RegisterRequest registerRequest)
    {
        return await Client.PostAsJsonAsync(BuildUrl(GetCurrentRouteUrl(), CurrentUrl, AppConstants.AppController.IdentityController.Register), registerRequest);
    }

    [Fact]
    public async Task Register_ShouldReturnSuccess()
    {
        // Arrange
        await EnsureTokenAsync();
        var registerRequest = IdentityFixture.GetRegisterRequest();

        // Act
        var response = await GetRegisteredUserResponseAsync(registerRequest);

        // Assert
        await AssertUserResponseIsValid(registerRequest, response);
    }

    private static async Task AssertUserResponseIsValid(RegisterRequest registerRequest, HttpResponseMessage response)
    {
        // Assert
        response.EnsureSuccessStatusCode();

        // Arrange
        var userResponseDto = await response.Content.
        ReadFromJsonAsync<UserRegisterResponse<string>?>();
        response.EnsureSuccessStatusCode();

        // Assert
        Assert.Equal(StatusCodes.Status201Created, (int)response.StatusCode);
        Assert.NotNull(userResponseDto);
        Assert.Equal(StatusCodes.Status201Created, userResponseDto.StatusCode);
        Assert.Equal("true", userResponseDto.Success);
        Assert.Equal(AppResponses.UserRegisterResponses.UserRegistered, userResponseDto.Message);
        Assert.NotNull(userResponseDto.User);
        Assert.Equal(registerRequest.FullName, userResponseDto.User.FullName);
        Assert.Equal(registerRequest.Email, userResponseDto.User.Email);
        Assert.Equal(registerRequest.PhoneNumber, userResponseDto.User.PhoneNumber);
        Assert.Equal(registerRequest.City, userResponseDto.User.City);
        Assert.Equal(registerRequest.AddressLine1, userResponseDto.User.AddressLine1);
        Assert.Equal(registerRequest.AddressLine2, userResponseDto.User.AddressLine2);
        Assert.Equal(registerRequest.Country, userResponseDto.User.Country);
        Assert.Equal(registerRequest.DayOfBirth, userResponseDto.User.DayOfBirth);
        Assert.Equal(registerRequest.PostCode, userResponseDto.User.PostCode);
        Assert.Equal(registerRequest.LicensePlates, userResponseDto.User.UserLicensePlates);
    }

    [Fact]
    public async Task ConfirmEmail_ShouldReturnSuccess_WhenTokenIsValid()
    {
        // Arrange
        var registerRequest = IdentityFixture.GetRegisterRequest();

        var registerResponse = await GetRegisteredUserResponseAsync(registerRequest);

        await AssertUserResponseIsValid(registerRequest, registerResponse);

        var user = await UserManager.FindByEmailAsync(registerRequest.Email!);
        Assert.NotNull(user);

        var token = await UserManager.GenerateEmailConfirmationTokenAsync(user);
        var based64Token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var confirmEmailUrl =
            $"{BuildUrl(GetCurrentRouteUrl(), CurrentUrl, AppConstants.AppController.IdentityController.ConfirmEmail)}?email={Uri.EscapeDataString(registerRequest.Email!)}&token={Uri.EscapeDataString(based64Token)}";

        // Act
        var response = await Client.GetAsync(confirmEmailUrl);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<ConfirmEmailResponse<string>>();
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal("true", result.Success);
        Assert.Equal(AppResponses.EmailConfirmationResponses.EmailConfirmed, result.Message);
    }
}