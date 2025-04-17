using System.Net.Http.Json;
using ApiTests.Fixtures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Secure_P_Backend;
using SecureP.Identity.Models;
using SecureP.Identity.Models.Dto;
using SecureP.Service.Abstraction.Entities;
using SecureP.Shared;

namespace ApiTests;

[Collection(nameof(ParkingRateCollection))]
public class ParkingRateControllerTests : ApiControllerTests
{
    public new string CurrentUrl { get; set; } = "parking-rate";
    public List<ParkingRate<string>>? ParkingRates { get; set; } = default!;

    public ParkingRateControllerTests(TestApplicationFactory factory, ParkingRateFixture fixture) : base(factory)
    {
        ParkingRates = fixture.ParkingRates;
    }

    [Fact]
    public async Task GetParkingRates_ShouldReturnOk()
    {
        // Arrange //Act // Assert
        await EnsureDataArrangementAsync();
    }

    [Fact]
    public async Task GetParkingRateById_ShouldReturnOk()
    {
        // Arrange
        await EnsureDataArrangementAsync();

        // Act
        var response = await Client.GetAsync(BuildUrl(GetCurrentRouteUrl(), CurrentUrl, ParkingRates?.FirstOrDefault()?.Id ?? throw new ArgumentNullException(nameof(ParkingRates))));
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        var parkingRate = (await response.Content.ReadFromJsonAsync<GetParkingRateByIdResponse<string>>())?.Data;

        // Assert
        Assert.NotNull(parkingRate);
        Assert.Equal(ParkingRates?.FirstOrDefault()?.Id, parkingRate?.Id);
        Assert.Equal(ParkingRates?.FirstOrDefault()?.HourlyRate, parkingRate?.HourlyRate);
        Assert.Equal(ParkingRates?.FirstOrDefault()?.DailyRate, parkingRate?.DailyRate);
        Assert.Equal(ParkingRates?.FirstOrDefault()?.MonthlyRate, parkingRate?.MonthlyRate);
        Assert.Equal(ParkingRates?.FirstOrDefault()?.ConcurrencyStamp, parkingRate?.ConcurrencyStamp);
    }

    [Fact]
    public async Task CreateParkingRate_ShouldReturnCreated()
    {
        // Arrange // Act // Assert
        await EnsureDataArrangementAsync();
    }

    [Fact]
    public async Task UpdateParkingRate_WithCorrectConcurrencyStamp_ShouldReturnOk()
    {
        // Arrange
        await EnsureDataArrangementAsync();
        var parkingRate = ParkingRates?.FirstOrDefault() ?? throw new ArgumentNullException(nameof(ParkingRates));
        var updateParkingRateRequest = new UpdateParkingRateRequest<string>
        {
            Id = parkingRate.Id,
            HourlyRate = 8.0,
            DailyRate = 35.0,
            MonthlyRate = 200.0,
            ConcurrencyStamp = parkingRate.ConcurrencyStamp
        };

        // Act
        var updateResponse = await Client.PutAsJsonAsync(BuildUrl(GetCurrentRouteUrl(), CurrentUrl, parkingRate.Id), updateParkingRateRequest);
        Assert.True(updateResponse.IsSuccessStatusCode);
        Assert.Equal(StatusCodes.Status200OK, (int)updateResponse.StatusCode);

        var getParkingRateByIdResponse = await Client.GetAsync(BuildUrl(GetCurrentRouteUrl(), CurrentUrl, parkingRate.Id));
        Assert.True(getParkingRateByIdResponse.IsSuccessStatusCode);
        Assert.Equal(StatusCodes.Status200OK, (int)getParkingRateByIdResponse.StatusCode);
        var retrieveParkingRate = (await getParkingRateByIdResponse.Content.ReadFromJsonAsync<GetParkingRateByIdResponse<string>>())?.Data;

        // Assert
        var content = await updateResponse.Content.ReadFromJsonAsync<UpdateParkingRateResponse>();
        Assert.True(content?.Success);
        Assert.Equal(StatusCodes.Status200OK, content?.StatusCode);
        Assert.Equal(AppResponses.UpdateParkingRateResponses.ParkingRateUpdated, content?.Message);

        Assert.Equal(retrieveParkingRate?.Id, parkingRate.Id);
        Assert.Equal(retrieveParkingRate?.HourlyRate, updateParkingRateRequest.HourlyRate);
        Assert.Equal(retrieveParkingRate?.DailyRate, updateParkingRateRequest.DailyRate);
        Assert.Equal(retrieveParkingRate?.MonthlyRate, updateParkingRateRequest.MonthlyRate);
        Assert.NotEqual(parkingRate.ConcurrencyStamp, retrieveParkingRate?.ConcurrencyStamp);
    }

    [Fact]
    public async Task UpdateParkingRate_WithIncorrectConcurrencyStamp_ShouldReturnConflict()
    {
        // Arrange
        await EnsureDataArrangementAsync();
        var parkingRate = ParkingRates?.FirstOrDefault() ?? throw new ArgumentNullException(nameof(ParkingRates));
        var updateParkingRateRequest = new UpdateParkingRateRequest<string>
        {
            Id = parkingRate.Id,
            HourlyRate = 8.0,
            DailyRate = 35.0,
            MonthlyRate = 200.0,
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        // Act
        var updateResponse = await Client.PutAsJsonAsync(BuildUrl(GetCurrentRouteUrl(), CurrentUrl, parkingRate.Id), updateParkingRateRequest);
        Assert.False(updateResponse.IsSuccessStatusCode);
        Assert.Equal(StatusCodes.Status400BadRequest, (int)updateResponse.StatusCode);

        // Assert
        var content = await updateResponse.Content.ReadFromJsonAsync<UpdateParkingRateResponse>();
        Assert.False(content?.Success);
        Assert.Equal(StatusCodes.Status400BadRequest, content?.StatusCode);
        Assert.NotNull(content?.Errors);
        Assert.Empty(content?.Errors ?? []);
    }

    [Fact]
    public async Task DeleteParkingRate_ShouldReturnNoContent()
    {
        // Arrange
        await EnsureDataArrangementAsync();
        var parkingRate = ParkingRates?.FirstOrDefault() ?? throw new ArgumentNullException(nameof(ParkingRates));

        // Act
        var deleteResponse = await Client.DeleteAsync(BuildUrl(GetCurrentRouteUrl(), CurrentUrl, parkingRate.Id));
        Assert.True(deleteResponse.IsSuccessStatusCode);
        Assert.Equal(StatusCodes.Status200OK, (int)deleteResponse.StatusCode);

        var deleteResponseContent = await deleteResponse.Content.ReadFromJsonAsync<DeleteParkingRateResponse>();
        Assert.True(deleteResponseContent?.Success);
        Assert.Equal(StatusCodes.Status204NoContent, deleteResponseContent?.StatusCode);
        Assert.Equal(AppResponses.DeleteParkingRateResponses.ParkingRateDeleted, deleteResponseContent?.Message);

        var getResponse = await Client.GetAsync(BuildUrl(GetCurrentRouteUrl(), CurrentUrl, parkingRate.Id));
        Assert.False(getResponse.IsSuccessStatusCode);
        Assert.Equal(StatusCodes.Status404NotFound, (int)getResponse.StatusCode);
        var getResponseContent = await getResponse.Content.ReadFromJsonAsync<DeleteParkingRateResponse>();
        Assert.False(getResponseContent?.Success);
        Assert.Equal(StatusCodes.Status404NotFound, getResponseContent?.StatusCode);
        Assert.Equal(AppResponses.GetParkingRateResponses.ParkingRateNotFound, getResponseContent?.Message);

        ParkingRates?.Remove(parkingRate);
    }

    private async Task EnsureDataArrangementAsync()
    {
        await EnsureTokenAsync();
        await EnsureCreateParkingRateAsync();
        await EnsureRetrieveParkingRatesAsync();
    }

    private async Task EnsureCreateParkingRateAsync()
    {
        if (ParkingRates != null && ParkingRates.Any())
        {
            return; // Parking rates already exist, no need to create them again
        }

        ParkingRates ??= [];
        var parkingRates = new List<CreateParkingRateRequest>
        {
            new() {
                HourlyRate = 5.0,
                DailyRate = 20.0,
                MonthlyRate = 100.0,
            },
            new() {
                HourlyRate = 6.0,
                DailyRate = 25.0,
                MonthlyRate = 120.0,
            },
            new() {
                HourlyRate = 7.0,
                DailyRate = 30.0,
                MonthlyRate = 150.0,
            },
        };

        foreach (var parkingRate in parkingRates)
        {
            var response = await Client.PostAsJsonAsync(BuildUrl(GetCurrentRouteUrl(), CurrentUrl), parkingRate);
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(StatusCodes.Status201Created, (int)response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<CreateParkingRateResponse>();
            Assert.True(content?.Success);
            Assert.Equal(StatusCodes.Status201Created, content?.StatusCode);
            Assert.Equal(AppResponses.CreateParkingRateResponses.ParkingRateCreated, content?.Message);
        }
    }

    private async Task EnsureRetrieveParkingRatesAsync()
    {
        if (ParkingRates != null && ParkingRates.Any())
        {
            return; // Parking rates already exist, no need to get them again
        }

        var getResponse = await Client.GetAsync(BuildUrl(GetCurrentRouteUrl(), CurrentUrl));
        Assert.True(getResponse.IsSuccessStatusCode);
        Assert.Equal(StatusCodes.Status200OK, (int)getResponse.StatusCode);

        var content = await getResponse.Content.ReadFromJsonAsync<GetAllParkingRateResponse<string>>();
        ParkingRates?.AddRange(content?.Data?.Items?.Select(x => new ParkingRate<string>
        {
            Id = x.Id,
            HourlyRate = x.HourlyRate,
            DailyRate = x.DailyRate,
            MonthlyRate = x.MonthlyRate,
            ConcurrencyStamp = x.ConcurrencyStamp
        }) ?? []);

        Assert.True(content?.Success);
        Assert.NotNull(ParkingRates);
        Assert.NotEmpty(ParkingRates);
        Assert.Equal(3, ParkingRates.Count());
        Assert.Equal(StatusCodes.Status200OK, content?.StatusCode);
        Assert.Equal(AppResponses.GetAllParkingRateResponses.ParkingRatesFound, content?.Message);
        Assert.Equal(1, content?.Data?.TotalPages);
        Assert.Equal(3, content?.Data?.TotalItems);
    }
}