using SecureP.Identity.Models.Dto;
using SecureP.Identity.Models.Dto.SortModels;
using SecureP.Identity.Models.Result;
using SecureP.Service.Abstraction.Entities;
using ServiceTests.Fixtures;

namespace ServiceTests;

public class ParkingZoneServiceTest : IClassFixture<SqlServerTestDbFixture<string>>
{
    public SqlServerTestDbFixture<string> Fixture { get; }
    public ParkingZoneServiceTest(SqlServerTestDbFixture<string> fixture)
    {
        Fixture = fixture;
    }

    [Fact]
    public async Task CreateAsync_Should_Insert_New_ParkingZone()
    {
        // Arrange
        var newParkingRate = new CreateParkingRateDto<string>
        {
            Id = Guid.NewGuid().ToString(),
            HourlyRate = (double)1.0m,
            DailyRate = (double)10.0m,
            MonthlyRate = (double)50.0m,
        };
        var (_, createdParkingRate) = await Fixture.ParkingRateService.CreateParkingRateAsync(newParkingRate);

        var newParkingLocation = new CreateParkingLocationRequest
        {
            Name = "Test Parking Location",
            Address = "123 Test St",
            ParkingZones = [],
            ParkingRateId = createdParkingRate?.Id ?? default!,
        };
        var (_, createdParkingLocation) = await Fixture.ParkingLocationService.CreateParkingLocationAsync(newParkingLocation);

        var newId = Guid.NewGuid().ToString();
        var newCreateParkingZoneDto = new CreateParkingZoneDto<string>
        {
            Id = newId,
            Name = "Test Parking Zone",
            Capacity = 100,
            AvailableSpaces = 100,
            ParkingLocationId = createdParkingLocation?.Id ?? default!,
        };

        // Act
        var (validationResult, createdParkingZoneDto) = await Fixture.ParkingZoneService.CreateParkingZoneAsync(newCreateParkingZoneDto);

        // Assert
        Assert.NotNull(validationResult);
        Assert.True(validationResult.Success, "ValidationResult should be successful.");

        Assert.NotNull(createdParkingZoneDto);

        Assert.NotNull(createdParkingZoneDto.Id);
        Assert.Equal(newId, createdParkingZoneDto.Id);

        Assert.Equal(newCreateParkingZoneDto.Name, createdParkingZoneDto.Name);

        Assert.Equal(newCreateParkingZoneDto.Capacity, createdParkingZoneDto.Capacity);

        Assert.Equal(newCreateParkingZoneDto.AvailableSpaces, createdParkingZoneDto.AvailableSpaces);

        Assert.NotNull(createdParkingZoneDto.Id);

        Assert.True(Guid.TryParse(createdParkingZoneDto.Id, out _), "Id should be a valid Guid.");

        Assert.NotNull(createdParkingZoneDto.ConcurrencyStamp);

        Assert.True(Guid.TryParse(createdParkingZoneDto.ConcurrencyStamp, out _), "ConcurrencyStamp should be a valid Guid.");

        Assert.Equal(newCreateParkingZoneDto.ParkingLocationId, createdParkingZoneDto.ParkingLocationId);
    }

    [Fact]
    public async Task CreateAsync_Should_Fail_On_AllFour_TestCases()
    {
        // Arrange
        var newParkingRate = new CreateParkingRateDto<string>
        {
            Id = Guid.NewGuid().ToString(),
            HourlyRate = (double)1.0m,
            DailyRate = (double)10.0m,
            MonthlyRate = (double)50.0m,
        };
        var (_, createdParkingRate) = await Fixture.ParkingRateService.CreateParkingRateAsync(newParkingRate);

        var newParkingLocation = new CreateParkingLocationRequest
        {
            Name = "Test Parking Location",
            Address = "123 Test St",
            ParkingZones = [],
            ParkingRateId = createdParkingRate?.Id ?? default!,
        };
        var (_, createdParkingLocation) = await Fixture.ParkingLocationService.CreateParkingLocationAsync(newParkingLocation);

        List<CreateParkingZoneDto<string>> testCases = [
            new CreateParkingZoneDto<string> { Id = null!, Name = "Parking Zone 1", Capacity = 1, AvailableSpaces = 1, ParkingLocationId = createdParkingLocation?.Id }, // Invalid Id (default value)
            new CreateParkingZoneDto<string> { Id = Guid.NewGuid().ToString(), Name = "Parking Zone 2", Capacity = 0, AvailableSpaces = 1, ParkingLocationId = createdParkingLocation?.Id }, // Invalid Capacity (Less than 1)
            new CreateParkingZoneDto<string> { Id = Guid.NewGuid().ToString(), Name = "Parking Zone 3", Capacity = 1, AvailableSpaces = -1, ParkingLocationId = createdParkingLocation?.Id }, // Invalid Available Spaces (Less than 0)
            new CreateParkingZoneDto<string> { Id = Guid.NewGuid().ToString(), Name = "Parking Zone 4", Capacity = 1, AvailableSpaces = 2, ParkingLocationId = createdParkingLocation?.Id }, // Invalid Available Spaces (Greater than Capacity)            
        ];

        // Act
        List<ValidationResult> validationResults = [];
        List<CreatedParkingZoneDto<string>?> returnObjects = [];
        foreach (var testCase in testCases)
        {
            var (validationResult, returnObject) = await Fixture.ParkingZoneService.CreateParkingZoneAsync(testCase);
            validationResults.Add(validationResult);
            returnObjects.Add(returnObject);
        }

        // Assert
        Assert.All(validationResults, result => Assert.False(result.Success, "ValidationResult should not be successful."));
        Assert.All(returnObjects, Assert.Null);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_ParkingZones_With_Filter_And_Sort()
    {
        // Arrange
        var newParkingRate = new CreateParkingRateDto<string>
        {
            Id = Guid.NewGuid().ToString(),
            HourlyRate = (double)1.0m,
            DailyRate = (double)10.0m,
            MonthlyRate = (double)50.0m,
        };
        var (_, createdParkingRate) = await Fixture.ParkingRateService.CreateParkingRateAsync(newParkingRate);

        var newParkingLocation = new CreateParkingLocationRequest
        {
            Name = "Test Parking Location",
            Address = "123 Test St",
            ParkingZones = [],
            ParkingRateId = createdParkingRate?.Id ?? default!,
        };
        var (_, createdParkingLocation) = await Fixture.ParkingLocationService.CreateParkingLocationAsync(newParkingLocation);

        var newParkingZone1 = new CreateParkingZoneDto<string>
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test Parking Zone 1",
            Capacity = 100,
            AvailableSpaces = 100,
            ParkingLocationId = createdParkingLocation?.Id ?? default!,
        };

        var newParkingZone2 = new CreateParkingZoneDto<string>
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test Parking Zone 2",
            Capacity = 100,
            AvailableSpaces = 100,
            ParkingLocationId = createdParkingLocation?.Id ?? default!,
        };

        await Fixture.ParkingZoneService.CreateParkingZoneAsync(newParkingZone1);
        await Fixture.ParkingZoneService.CreateParkingZoneAsync(newParkingZone2);

        // Act
        var parkingRate = await Fixture.ParkingZoneService.GetParkingZonesAsync();
        var parkingRate2 = await Fixture.ParkingZoneService.GetParkingZonesAsync(page: 1, limit: 2, sort: ParkingZoneOrderBy.Name, desc: true, search: "2");
        var parkingZone3 = await Fixture.ParkingZoneService.GetParkingZonesAsync(desc: true);

        // Assert
        Assert.NotNull(parkingRate);
        Assert.Equal(2, parkingRate.Items.Count);
        Assert.Equal(1, parkingRate.TotalPages);
        Assert.Equal(2, parkingRate.TotalItems);
        Assert.Contains(parkingRate.Items, x => x.Name == newParkingZone1.Name && x.Capacity == newParkingZone1.Capacity && x.AvailableSpaces == newParkingZone1.AvailableSpaces && x.Id == newParkingZone1.Id && x.ParkingLocationId == newParkingZone1.ParkingLocationId);
        Assert.Contains(parkingRate.Items, x => x.Name == newParkingZone2.Name && x.Capacity == newParkingZone2.Capacity && x.AvailableSpaces == newParkingZone2.AvailableSpaces && x.Id == newParkingZone2.Id && x.ParkingLocationId == newParkingZone2.ParkingLocationId);

        Assert.NotNull(parkingRate2);
        Assert.Single(parkingRate2.Items);
        Assert.Equal(1, parkingRate2.TotalPages);
        Assert.Equal(2, parkingRate2.TotalItems);
        Assert.Contains(parkingRate2.Items, x => x.Name == newParkingZone2.Name && x.Capacity == newParkingZone2.Capacity && x.AvailableSpaces == newParkingZone2.AvailableSpaces && x.Id == newParkingZone2.Id && x.ParkingLocationId == newParkingZone2.ParkingLocationId);
        Assert.DoesNotContain(parkingRate2.Items, x => x.Name == newParkingZone1.Name && x.Capacity == newParkingZone1.Capacity && x.AvailableSpaces == newParkingZone1.AvailableSpaces && x.Id == newParkingZone1.Id && x.ParkingLocationId == newParkingZone1.ParkingLocationId);

        Assert.NotNull(parkingZone3);
        Assert.Equal(2, parkingZone3.Items.Count);
        Assert.Equal(2, parkingZone3.TotalItems);
        Assert.Equal(1, parkingZone3.TotalPages);
        Assert.Contains(parkingZone3.Items, x => x.Name == newParkingZone1.Name && x.Capacity == newParkingZone1.Capacity && x.AvailableSpaces == newParkingZone1.AvailableSpaces && x.Id == newParkingZone1.Id && x.ParkingLocationId == newParkingZone1.ParkingLocationId);
        Assert.Contains(parkingZone3.Items, x => x.Name == newParkingZone2.Name && x.Capacity == newParkingZone2.Capacity && x.AvailableSpaces == newParkingZone2.AvailableSpaces && x.Id == newParkingZone2.Id && x.ParkingLocationId == newParkingZone2.ParkingLocationId);
        Assert.Equal(2, parkingZone3.TotalItems);
        Assert.Collection(parkingZone3.Items,
            item =>
            {
                Assert.Equal(newParkingZone2.Name, item.Name);
                Assert.Equal(newParkingZone2.Capacity, item.Capacity);
                Assert.Equal(newParkingZone2.AvailableSpaces, item.AvailableSpaces);
                Assert.Equal(newParkingZone2.Id, item.Id);
                Assert.Equal(newParkingZone2.ParkingLocationId, item.ParkingLocationId);
            },
            item =>
            {
                Assert.Equal(newParkingZone1.Name, item.Name);
                Assert.Equal(newParkingZone1.Capacity, item.Capacity);
                Assert.Equal(newParkingZone1.AvailableSpaces, item.AvailableSpaces);
                Assert.Equal(newParkingZone1.Id, item.Id);
                Assert.Equal(newParkingZone1.ParkingLocationId, item.ParkingLocationId);
            });
    }

    [Fact]
    public async Task GetById_Should_Return_ParkingZone()
    {
        // Arrange
        var newParkingRate = new CreateParkingRateDto<string>
        {
            Id = Guid.NewGuid().ToString(),
            HourlyRate = (double)1.0m,
            DailyRate = (double)10.0m,
            MonthlyRate = (double)50.0m,
        };
        var (_, createdParkingRate) = await Fixture.ParkingRateService.CreateParkingRateAsync(newParkingRate);

        var newParkingLocation = new CreateParkingLocationRequest
        {
            Name = "Test Parking Location",
            Address = "123 Test St",
            ParkingZones = [],
            ParkingRateId = createdParkingRate?.Id ?? default!,
        };
        var (_, createdParkingLocation) = await Fixture.ParkingLocationService.CreateParkingLocationAsync(newParkingLocation);
        var newParkingZone = new CreateParkingZoneDto<string>
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test Parking Rate",
            Capacity = 100,
            AvailableSpaces = 100,
            ParkingLocationId = createdParkingLocation?.Id ?? default!,
        };
        await Fixture.ParkingZoneService.CreateParkingZoneAsync(newParkingZone);

        // Act
        var dbParkingZone = await Fixture.ParkingZoneService.GetParkingZoneByIdAsync(newParkingZone.Id);
        var dbParkingZone2 = await Fixture.ParkingZoneService.GetParkingZoneByIdAsync(Guid.NewGuid().ToString());

        // Assert
        Assert.NotNull(dbParkingZone);
        Assert.Equal(newParkingZone.Name, dbParkingZone.Name);
        Assert.Equal(newParkingZone.Capacity, dbParkingZone.Capacity);
        Assert.Equal(newParkingZone.AvailableSpaces, dbParkingZone.AvailableSpaces);
        Assert.NotNull(dbParkingZone.Id);
        Assert.Equal(newParkingZone.Id, dbParkingZone.Id);
        Assert.Equal(newParkingZone.ParkingLocationId, dbParkingZone.ParkingLocationId);
        Assert.NotNull(dbParkingZone.ConcurrencyStamp);
        Assert.True(Guid.TryParse(dbParkingZone.ConcurrencyStamp, out _), "ConcurrencyStamp should be a valid Guid.");

        Assert.Null(dbParkingZone2);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_ParkingRate()
    {
        // Arrange
        var newParkingRate = new CreateParkingRateDto<string>
        {
            Id = Guid.NewGuid().ToString(),
            HourlyRate = (double)1.0m,
            DailyRate = (double)10.0m,
            MonthlyRate = (double)50.0m,
        };
        var (_, createdParkingRate) = await Fixture.ParkingRateService.CreateParkingRateAsync(newParkingRate);

        var newParkingLocation = new CreateParkingLocationRequest
        {
            Name = "Test Parking Location",
            Address = "123 Test St",
            ParkingZones = [],
            ParkingRateId = createdParkingRate?.Id ?? default!,
        };
        var (_, createdParkingLocation) = await Fixture.ParkingLocationService.CreateParkingLocationAsync(newParkingLocation);
        var newParkingZone = new CreateParkingZoneDto<string>
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test Parking Zone",
            Capacity = 100,
            AvailableSpaces = 100,
            ParkingLocationId = createdParkingLocation?.Id ?? default!,
        };
        var (_, createdParkingZone) = await Fixture.ParkingZoneService.CreateParkingZoneAsync(newParkingZone);

        var updatedParkingZone = new UpdateParkingZoneDto<string>
        {
            Id = newParkingZone.Id,
            Name = "Updated Parking Zone",
            Capacity = 200,
            AvailableSpaces = 50,
            ParkingLocationId = null,
            ConcurrencyStamp = createdParkingZone?.ConcurrencyStamp ?? default!,
        };

        // Act
        var validationResult = await Fixture.ParkingZoneService.UpdateParkingZoneAsync(updatedParkingZone);
        var dbParkingZone = await Fixture.ParkingZoneService.GetParkingZoneByIdAsync(newParkingZone.Id);

        // Assert
        Assert.NotNull(validationResult);
        Assert.True(validationResult.Success, "ValidationResult should be successful.");

        Assert.NotNull(dbParkingZone);
        Assert.Equal(updatedParkingZone.Name, dbParkingZone.Name);
        Assert.Equal(updatedParkingZone.Capacity, dbParkingZone.Capacity);
        Assert.Equal(updatedParkingZone.AvailableSpaces, dbParkingZone.AvailableSpaces);
        Assert.Equal(updatedParkingZone.Id, dbParkingZone.Id);
        Assert.NotNull(dbParkingZone.ConcurrencyStamp);
        Assert.True(Guid.TryParse(dbParkingZone.ConcurrencyStamp, out _), "ConcurrencyStamp should be a valid Guid.");
    }


    [Fact]
    public async Task UpdateAsync_Should_Fail_On_Fourth_TestCases()
    {
        // Arrange
        var newParkingRate = new CreateParkingRateDto<string>
        {
            Id = Guid.NewGuid().ToString(),
            HourlyRate = (double)1.0m,
            DailyRate = (double)10.0m,
            MonthlyRate = (double)50.0m,
        };
        var (_, createdParkingRate) = await Fixture.ParkingRateService.CreateParkingRateAsync(newParkingRate);

        var updatedParkingRate1 = new UpdateParkingRateDto<string>
        {
            Id = newParkingRate.Id,
            HourlyRate = (double)2.0m,
            DailyRate = (double)20.0m,
            MonthlyRate = -2,
            ConcurrencyStamp = createdParkingRate?.ConcurrencyStamp ?? default!,
        };

        var updatedParkingRate2 = new UpdateParkingRateDto<string>
        {
            Id = newParkingRate.Id,
            HourlyRate = -1,
            DailyRate = (double)20.0m,
            MonthlyRate = (double)100.0m,
            ConcurrencyStamp = createdParkingRate?.ConcurrencyStamp ?? default!,
        };

        var updatedParkingRate3 = new UpdateParkingRateDto<string>
        {
            Id = newParkingRate.Id,
            HourlyRate = (double)2.0m,
            DailyRate = -10,
            MonthlyRate = (double)100.0m,
            ConcurrencyStamp = createdParkingRate?.ConcurrencyStamp ?? default!,
        };

        var updatedParkingRate4 = new UpdateParkingRateDto<string>
        {
            Id = newParkingRate.Id,
            HourlyRate = (double)2.0m,
            DailyRate = (double)20.0m,
            MonthlyRate = (double)100.0m,
            ConcurrencyStamp = Guid.NewGuid().ToString(), // Invalid ConcurrencyStamp
        };

        // Act
        var validationResult1 = await Fixture.ParkingRateService.UpdateParkingRateAsync(updatedParkingRate1);
        var validationResult2 = await Fixture.ParkingRateService.UpdateParkingRateAsync(updatedParkingRate2);
        var validationResult3 = await Fixture.ParkingRateService.UpdateParkingRateAsync(updatedParkingRate3);
        var validationResult4 = await Fixture.ParkingRateService.UpdateParkingRateAsync(updatedParkingRate4);

        // Assert
        Assert.NotNull(validationResult1);
        Assert.False(validationResult1.Success, "ValidationResult should not be successful.");

        Assert.NotNull(validationResult2);
        Assert.False(validationResult2.Success, "ValidationResult should not be successful.");

        Assert.NotNull(validationResult3);
        Assert.False(validationResult3.Success, "ValidationResult should not be successful.");

        Assert.NotNull(validationResult4);
        Assert.False(validationResult4.Success, "ValidationResult should not be successful.");
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_ParkingRate()
    {
        // Arrange// Arrange
        var newParkingRate = new CreateParkingRateDto<string>
        {
            Id = Guid.NewGuid().ToString(),
            HourlyRate = (double)1.0m,
            DailyRate = (double)10.0m,
            MonthlyRate = (double)50.0m,
        };
        var (_, createdParkingRate) = await Fixture.ParkingRateService.CreateParkingRateAsync(newParkingRate);

        var newParkingLocation = new CreateParkingLocationRequest
        {
            Name = "Test Parking Location",
            Address = "123 Test St",
            ParkingZones = [],
            ParkingRateId = createdParkingRate?.Id ?? default!,
        };
        var (_, createdParkingLocation) = await Fixture.ParkingLocationService.CreateParkingLocationAsync(newParkingLocation);
        var newParkingZone = new CreateParkingZoneDto<string>
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test Parking Rate",
            Capacity = 100,
            AvailableSpaces = 100,
            ParkingLocationId = createdParkingLocation?.Id ?? default!,
        };
        await Fixture.ParkingZoneService.CreateParkingZoneAsync(newParkingZone);

        // Act
        var deleteResult = await Fixture.ParkingZoneService.DeleteParkingZoneAsync(newParkingZone.Id);
        var deleteResult2 = await Fixture.ParkingZoneService.DeleteParkingZoneAsync(newParkingZone.Id);
        var dbParkingZone = await Fixture.ParkingZoneService.GetParkingZoneByIdAsync(newParkingZone.Id);

        // Assert
        Assert.True(deleteResult, "Delete result should be successful.");
        Assert.False(deleteResult2, "Delete result should not be successful.");

        Assert.Null(dbParkingZone);
    }
}
