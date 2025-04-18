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

[Collection(nameof(ParkingZoneCollection))]
public class ParkingZoneControllerTests : ApiControllerTests
{
    public new string CurrentUrl { get; set; } = "parking-zone";
    public virtual string ParkingZoneUrl { get; set; } = "parking-zone";
    public virtual string ParkingLocationUrl { get; set; } = "parking-location";
    public virtual string ParkingRateUrl { get; set; } = "parking-rate";
    public ParkingRate<string>? ParkingRate { get; set; } = default!;
    public ParkingLocation<string>? ParkingLocation { get; set; } = default!;
    public List<ParkingZone<string>>? ParkingZones { get; set; } = default!;


    public ParkingZoneControllerTests(TestApplicationFactory factory, ParkingZoneFixture fixture) : base(factory)
    {
        ParkingRate = fixture.ParkingRate;
        ParkingLocation = fixture.ParkingLocation;
        ParkingZones = fixture.ParkingZones;
    }

    [Fact]
    public async Task GetParkingZones_ShouldReturnOk()
    {
        // Arrange //Act // Assert
        await EnsureDataArrangementAsync();
    }

    [Fact]
    public async Task GetParkingZoneById_ShouldReturnOk()
    {
        // Arrange
        await EnsureDataArrangementAsync();

        // Act
        var response = await Client.GetAsync(BuildUrl(GetCurrentRouteUrl(), CurrentUrl, ParkingZones?.FirstOrDefault()?.Id ?? throw new ArgumentNullException(nameof(ParkingZones))));
        Assert.True(response.IsSuccessStatusCode);
        Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
        var parkingZone = (await response.Content.ReadFromJsonAsync<GetParkingZoneByIdResponse<string>>())?.Data;

        // Assert
        Assert.NotNull(parkingZone);
        Assert.Equal(ParkingZones?.FirstOrDefault()?.Id, parkingZone?.Id);
        Assert.Equal(ParkingZones?.FirstOrDefault()?.Name, parkingZone?.Name);
        Assert.Equal(ParkingZones?.FirstOrDefault()?.Capacity, parkingZone?.Capacity);
        Assert.Equal(ParkingZones?.FirstOrDefault()?.AvailableSpaces, parkingZone?.AvailableSpaces);
        Assert.Equal(ParkingZones?.FirstOrDefault()?.ConcurrencyStamp, parkingZone?.ConcurrencyStamp);
        Assert.Equal(ParkingZones?.FirstOrDefault()?.ParkingLocationId, parkingZone?.ParkingLocationId);
    }

    [Fact]
    public async Task CreateParkingRate_ShouldReturnCreated()
    {
        // Arrange // Act // Assert
        await EnsureDataArrangementAsync();
    }

    [Fact]
    public async Task UpdateParkingZone_WithCorrectConcurrencyStamp_ShouldReturnOk()
    {
        // Arrange
        await EnsureDataArrangementAsync();
        var parkingZone = ParkingZones?.FirstOrDefault() ?? throw new ArgumentNullException(nameof(ParkingZones));
        var updateParkingZoneRequest = new UpdateParkingZoneRequest<string>
        {
            Id = parkingZone.Id,
            Name = "Updated Zone",
            Capacity = 100,
            AvailableSpaces = 25,
            ParkingLocationId = null,
            ConcurrencyStamp = parkingZone?.ConcurrencyStamp ?? string.Empty
        };

        // Act
        var updateResponse = await Client.PutAsJsonAsync(BuildUrl(GetCurrentRouteUrl(), CurrentUrl, parkingZone!.Id), updateParkingZoneRequest);
        Assert.True(updateResponse.IsSuccessStatusCode);
        Assert.Equal(StatusCodes.Status200OK, (int)updateResponse.StatusCode);

        var getParkingZoneByIdResponse = await Client.GetAsync(BuildUrl(GetCurrentRouteUrl(), CurrentUrl, parkingZone.Id));
        Assert.True(getParkingZoneByIdResponse.IsSuccessStatusCode);
        Assert.Equal(StatusCodes.Status200OK, (int)getParkingZoneByIdResponse.StatusCode);
        var retrieveParkingZone = (await getParkingZoneByIdResponse.Content.ReadFromJsonAsync<GetParkingZoneByIdResponse<string>>())?.Data;
        var content = await updateResponse.Content.ReadFromJsonAsync<UpdateParkingZoneResponse>();

        // Assert
        Assert.True(content?.Success);
        Assert.Equal(StatusCodes.Status200OK, content?.StatusCode);
        Assert.Equal(AppResponses.UpdateParkingZoneResponses.ParkingZoneUpdated, content?.Message);

        Assert.Equal(retrieveParkingZone?.Id, parkingZone.Id);
        Assert.Equal(retrieveParkingZone?.Name, updateParkingZoneRequest.Name);
        Assert.Equal(retrieveParkingZone?.Capacity, updateParkingZoneRequest.Capacity);
        Assert.Equal(retrieveParkingZone?.AvailableSpaces, updateParkingZoneRequest.AvailableSpaces);
        Assert.Equal(retrieveParkingZone?.ParkingLocationId, updateParkingZoneRequest.ParkingLocationId);
        Assert.NotEqual(parkingZone.ConcurrencyStamp, retrieveParkingZone?.ConcurrencyStamp);
    }

    [Fact]
    public async Task UpdateParkingZone_WithIncorrectConcurrencyStamp_ShouldReturnConflict()
    {
        // Arrange
        await EnsureDataArrangementAsync();
        var parkingZone = ParkingZones?.FirstOrDefault() ?? throw new ArgumentNullException(nameof(ParkingZones));
        var updateParkingZoneRequest = new UpdateParkingZoneRequest<string>
        {
            Id = parkingZone.Id,
            Name = "Updated Zone",
            Capacity = 100,
            AvailableSpaces = 25,
            ParkingLocationId = null,
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        // Act
        var updateResponse = await Client.PutAsJsonAsync(BuildUrl(GetCurrentRouteUrl(), CurrentUrl, parkingZone.Id), updateParkingZoneRequest);
        Assert.False(updateResponse.IsSuccessStatusCode);
        Assert.Equal(StatusCodes.Status400BadRequest, (int)updateResponse.StatusCode);

        // Assert
        var content = await updateResponse.Content.ReadFromJsonAsync<UpdateParkingZoneResponse>();
        Assert.False(content?.Success);
        Assert.Equal(StatusCodes.Status400BadRequest, content?.StatusCode);
        Assert.NotNull(content?.Errors);
        Assert.Empty(content?.Errors ?? []);
    }

    [Fact]
    public async Task DeleteParkingZone_ShouldReturnNoContent()
    {
        // Arrange
        await EnsureDataArrangementAsync();
        var parkingZone = ParkingZones?.FirstOrDefault() ?? throw new ArgumentNullException(nameof(ParkingZones));

        // Act
        var deleteResponse = await Client.DeleteAsync(BuildUrl(GetCurrentRouteUrl(), CurrentUrl, parkingZone.Id));
        Assert.True(deleteResponse.IsSuccessStatusCode);
        Assert.Equal(StatusCodes.Status200OK, (int)deleteResponse.StatusCode);

        var deleteResponseContent = await deleteResponse.Content.ReadFromJsonAsync<DeleteParkingZoneResponse>();
        Assert.True(deleteResponseContent?.Success);
        Assert.Equal(StatusCodes.Status204NoContent, deleteResponseContent?.StatusCode);
        Assert.Equal(AppResponses.DeleteParkingZoneResponses.ParkingZoneDeleted, deleteResponseContent?.Message);

        var getResponse = await Client.GetAsync(BuildUrl(GetCurrentRouteUrl(), CurrentUrl, parkingZone.Id));
        Assert.False(getResponse.IsSuccessStatusCode);
        Assert.Equal(StatusCodes.Status404NotFound, (int)getResponse.StatusCode);
        var getResponseContent = await getResponse.Content.ReadFromJsonAsync<DeleteParkingZoneResponse>();
        Assert.False(getResponseContent?.Success);
        Assert.Equal(StatusCodes.Status404NotFound, getResponseContent?.StatusCode);
        Assert.Equal(AppResponses.GetParkingZoneResponses.ParkingZoneNotFound, getResponseContent?.Message);

        ParkingZones?.Remove(parkingZone);
    }

    private async Task EnsureDataArrangementAsync()
    {
        await EnsureTokenAsync();
        await EnsureCreateParkingRateAsync();
        await EnsureRetrieveParkingZonesAsync();
    }

    private async Task EnsureCreateParkingRateAsync()
    {
        if (ParkingRate is null)
        {
            var parkingRate = new CreateParkingRateRequest
            {
                HourlyRate = 5.0,
                DailyRate = 20.0,
                MonthlyRate = 100.0,
            };

            var response = await Client.PostAsJsonAsync(BuildUrl(GetCurrentRouteUrl(), ParkingRateUrl), parkingRate);
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(StatusCodes.Status201Created, (int)response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<CreateParkingRateResponse>();
            Assert.True(content?.Success);
            Assert.Equal(StatusCodes.Status201Created, content?.StatusCode);
            Assert.Equal(AppResponses.CreateParkingRateResponses.ParkingRateCreated, content?.Message);

            ParkingRate = new ParkingRate<string>
            {
                Id = response.Headers.Location?.Segments.LastOrDefault() ?? throw new ArgumentNullException(nameof(response.Headers.Location)),
                HourlyRate = parkingRate.HourlyRate,
                DailyRate = parkingRate.DailyRate,
                MonthlyRate = parkingRate.MonthlyRate,
            };
        }

        if (ParkingLocation is null)
        {
            var parkingLocation = new CreateParkingLocationRequest
            {
                Name = "Test Location",
                Address = "123 Test St",
                ParkingZones = [],
                ParkingRateId = ParkingRate.Id,
            };

            var response = await Client.PostAsJsonAsync(BuildUrl(GetCurrentRouteUrl(), ParkingLocationUrl), parkingLocation);
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(StatusCodes.Status201Created, (int)response.StatusCode);

            var content = await response.Content.ReadFromJsonAsync<CreateParkingLocationResponse>();
            Assert.True(content?.Success);
            Assert.Equal(StatusCodes.Status201Created, content?.StatusCode);
            Assert.Equal(AppResponses.CreateParkingLocationResponses.ParkingLocationCreated, content?.Message);

            ParkingLocation = new ParkingLocation<string>
            {
                Id = response.Headers.Location?.Segments.LastOrDefault() ?? throw new ArgumentNullException(nameof(response.Headers.Location)),
                Name = parkingLocation.Name,
                Address = parkingLocation.Address,
            };
        }

        if (ParkingZones is null || ParkingZones?.Count == 0)
        {
            ParkingZones ??= [];
            var parkingZones = new List<CreateParkingZoneRequest<string>>
            {
                new() {
                    Name = "Test Zone 1",
                    AvailableSpaces = 10,
                    Capacity = 20,
                    ParkingLocationId = ParkingLocation.Id,
                },
                new() {
                    Name = "Test Zone 2",
                    AvailableSpaces = 15,
                    Capacity = 30,
                    ParkingLocationId = ParkingLocation.Id,
                },
                new() {
                    Name = "Test Zone 3",
                    AvailableSpaces = 5,
                    Capacity = 10,
                    ParkingLocationId = ParkingLocation.Id,
                },
            };

            foreach (var parkingZone in parkingZones)
            {
                var response = await Client.PostAsJsonAsync(BuildUrl(GetCurrentRouteUrl(), CurrentUrl), parkingZone);
                Assert.True(response.IsSuccessStatusCode);
                Assert.Equal(StatusCodes.Status201Created, (int)response.StatusCode);

                var content = await response.Content.ReadFromJsonAsync<CreateParkingRateResponse>();
                Assert.True(content?.Success);
                Assert.Equal(StatusCodes.Status201Created, content?.StatusCode);
                Assert.Equal(AppResponses.CreateParkingRateResponses.ParkingRateCreated, content?.Message);
            }
        }
    }

    private async Task EnsureRetrieveParkingZonesAsync()
    {
        if (ParkingZones != null && ParkingZones.Count != 0)
        {
            return; // Parking rates already exist, no need to get them again
        }

        var getResponse = await Client.GetAsync(BuildUrl(GetCurrentRouteUrl(), CurrentUrl));
        Assert.True(getResponse.IsSuccessStatusCode);
        Assert.Equal(StatusCodes.Status200OK, (int)getResponse.StatusCode);

        var content = await getResponse.Content.ReadFromJsonAsync<GetAllParkingZoneResponse<string>>();
        ParkingZones?.Clear();
        ParkingZones?.AddRange(content?.Data?.Items?.Select(x => new ParkingZone<string>
        {
            Id = x.Id,
            Name = x.Name,
            AvailableSpaces = x.AvailableSpaces,
            Capacity = x.Capacity,
            ParkingLocationId = x.ParkingLocationId,
            ConcurrencyStamp = x.ConcurrencyStamp
        }) ?? []);

        Assert.True(content?.Success);
        Assert.NotNull(ParkingZones);
        Assert.NotEmpty(ParkingZones);
        Assert.Equal(3, ParkingZones.Count());
        Assert.Equal(StatusCodes.Status200OK, content?.StatusCode);
        Assert.Equal(AppResponses.GetAllParkingZoneResponses.ParkingZonesFound, content?.Message);
        Assert.Equal(1, content?.Data?.TotalPages);
        Assert.Equal(3, content?.Data?.TotalItems);
    }
}