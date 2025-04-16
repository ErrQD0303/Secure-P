using RepositoryTests.Fixtures;
using SecureP.Identity.Models.Dto;
using SecureP.Identity.Models.Dto.SortModels;
using SecureP.Identity.Models.Result;
using SecureP.Service.Abstraction.Entities;

namespace ServiceTests;

public class ParkingRateServiceTests : IClassFixture<SqlServerTestDbFixture<string>>
{
    public SqlServerTestDbFixture<string> Fixture { get; }
    public ParkingRateServiceTests(SqlServerTestDbFixture<string> fixture)
    {
        Fixture = fixture;
    }

    [Fact]
    public async Task CreateAsync_Should_Insert_New_ParkingRate()
    {
        // Arrange
        var newId = Guid.NewGuid().ToString();
        var newCreateParkingRateDto = new CreateParkingRateDto<string>
        {
            Id = newId,
            HourlyRate = (double)1.0m,
            DailyRate = (double)10.0m,
            MonthlyRate = (double)50.0m,
        };

        // Act
        var (validationResult, createdParkingRateDto) = await Fixture.ParkingRateService.CreateParkingRateAsync(newCreateParkingRateDto);

        // Assert
        Assert.NotNull(validationResult);
        Assert.True(validationResult.Success, "ValidationResult should be successful.");

        Assert.NotNull(createdParkingRateDto);

        Assert.NotNull(createdParkingRateDto.Id);
        Assert.Equal(newId, createdParkingRateDto.Id);

        Assert.Equal(newCreateParkingRateDto.HourlyRate, createdParkingRateDto.HourlyRate);

        Assert.Equal(newCreateParkingRateDto.DailyRate, createdParkingRateDto.DailyRate);

        Assert.Equal(newCreateParkingRateDto.MonthlyRate, createdParkingRateDto.MonthlyRate);

        Assert.NotNull(createdParkingRateDto.Id);

        Assert.True(Guid.TryParse(createdParkingRateDto.Id, out _), "Id should be a valid Guid.");

        Assert.NotNull(createdParkingRateDto.ConcurrencyStamp);

        Assert.True(Guid.TryParse(createdParkingRateDto.ConcurrencyStamp, out _), "ConcurrencyStamp should be a valid Guid.");

        // Clean up
        Fixture.Context.ParkingRates.RemoveRange(Fixture.Context.ParkingRates);
    }

    [Fact]
    public async Task CreateAsync_Should_Fail_On_Three_TestCases()
    {
        // Arrange
        List<CreateParkingRateDto<string>> testCases = [
            new CreateParkingRateDto<string> { Id = null, HourlyRate = 1, DailyRate = 10, MonthlyRate = 50 }, // Invalid Id (default value)
            new CreateParkingRateDto<string> { Id = Guid.NewGuid().ToString(),HourlyRate = -1, DailyRate = 10, MonthlyRate = 50 }, // Negative HourlyRate
            new CreateParkingRateDto<string> { Id = Guid.NewGuid().ToString(),HourlyRate = 1, DailyRate = -10, MonthlyRate = 50 }, // Negative DailyRate
            new CreateParkingRateDto<string> { Id = Guid.NewGuid().ToString(),HourlyRate = 1, DailyRate = 10, MonthlyRate = -50 }, // Negative MonthlyRate
        ];

        // Act
        List<ValidationResult> validationResults = [];
        List<CreatedParkingRateDto<string>?> returnObjects = [];
        foreach (var testCase in testCases)
        {
            var (validationResult, returnObject) = await Fixture.ParkingRateService.CreateParkingRateAsync(testCase);
            validationResults.Add(validationResult);
            returnObjects.Add(returnObject);
        }

        // Assert
        Assert.All(validationResults, result => Assert.False(result.Success, "ValidationResult should not be successful."));
        Assert.All(returnObjects, returnObject => Assert.Null(returnObject));

        // Clean up
        Fixture.Context.ParkingRates.RemoveRange(Fixture.Context.ParkingRates);
    }

    [Fact]
    public async Task GetAllAsync_Should_Return_All_ParkingRates_With_Filter_And_Sort()
    {
        // Arrange
        Fixture.Context.ParkingRates.RemoveRange(Fixture.Context.ParkingRates);
        var newParkingRate1 = new CreateParkingRateDto<string>
        {
            Id = Guid.NewGuid().ToString(),
            HourlyRate = (double)1.0m,
            DailyRate = (double)10.0m,
            MonthlyRate = (double)50.0m,
        };

        var newParkingRate2 = new CreateParkingRateDto<string>
        {
            Id = Guid.NewGuid().ToString(),
            HourlyRate = (double)2.0m,
            DailyRate = (double)20.0m,
            MonthlyRate = (double)100.0m,
        };

        await Fixture.ParkingRateService.CreateParkingRateAsync(newParkingRate1);
        await Fixture.ParkingRateService.CreateParkingRateAsync(newParkingRate2);

        // Act
        var parkingRate = await Fixture.ParkingRateService.GetParkingRatesAsync();
        var parkingRate2 = await Fixture.ParkingRateService.GetParkingRatesAsync(page: 1, limit: 2, sort: ParkingRateOrderBy.MonthlyRate, desc: true, search: "2");
        var parkingRate3 = await Fixture.ParkingRateService.GetParkingRatesAsync(desc: true);

        // Assert
        Assert.NotNull(parkingRate);
        Assert.Equal(2, parkingRate.Items.Count);
        Assert.Equal(1, parkingRate.TotalPages);
        Assert.Equal(2, parkingRate.TotalItems);
        Assert.Contains(parkingRate.Items, x => x.HourlyRate == newParkingRate1.HourlyRate && x.DailyRate == newParkingRate1.DailyRate && x.MonthlyRate == newParkingRate1.MonthlyRate && x.Id == newParkingRate1.Id);
        Assert.Contains(parkingRate.Items, x => x.HourlyRate == newParkingRate2.HourlyRate && x.DailyRate == newParkingRate2.DailyRate && x.MonthlyRate == newParkingRate2.MonthlyRate && x.Id == newParkingRate2.Id);

        Assert.NotNull(parkingRate2);
        Assert.Single(parkingRate2.Items);
        Assert.Equal(1, parkingRate2.TotalPages);
        Assert.Equal(2, parkingRate2.TotalItems);
        Assert.Contains(parkingRate2.Items, x => x.HourlyRate == newParkingRate2.HourlyRate && x.DailyRate == newParkingRate2.DailyRate && x.MonthlyRate == newParkingRate2.MonthlyRate && x.Id == newParkingRate2.Id);
        Assert.DoesNotContain(parkingRate2.Items, x => x.HourlyRate == newParkingRate1.HourlyRate && x.DailyRate == newParkingRate1.DailyRate && x.MonthlyRate == newParkingRate1.MonthlyRate && x.Id == newParkingRate1.Id);

        Assert.NotNull(parkingRate3);
        Assert.Equal(2, parkingRate3.Items.Count);
        Assert.Equal(2, parkingRate3.TotalItems);
        Assert.Equal(1, parkingRate3.TotalPages);
        Assert.Contains(parkingRate3.Items, x => x.HourlyRate == newParkingRate1.HourlyRate && x.DailyRate == newParkingRate1.DailyRate && x.MonthlyRate == newParkingRate1.MonthlyRate && x.Id == newParkingRate1.Id);
        Assert.Contains(parkingRate3.Items, x => x.HourlyRate == newParkingRate2.HourlyRate && x.DailyRate == newParkingRate2.DailyRate && x.MonthlyRate == newParkingRate2.MonthlyRate && x.Id == newParkingRate2.Id);
        Assert.Collection(parkingRate3.Items,
            item =>
            {
                Assert.Equal(newParkingRate2.HourlyRate, item.HourlyRate);
                Assert.Equal(newParkingRate2.DailyRate, item.DailyRate);
                Assert.Equal(newParkingRate2.MonthlyRate, item.MonthlyRate);
            },
            item =>
            {
                Assert.Equal(newParkingRate1.HourlyRate, item.HourlyRate);
                Assert.Equal(newParkingRate1.DailyRate, item.DailyRate);
                Assert.Equal(newParkingRate1.MonthlyRate, item.MonthlyRate);
            });

        // Clean up
        Fixture.Context.ParkingRates.RemoveRange(Fixture.Context.ParkingRates);
    }

    [Fact]
    public async Task GetById_Should_Return_ParkingRate()
    {
        // Arrange
        var newParkingRate = new CreateParkingRateDto<string>
        {
            Id = Guid.NewGuid().ToString(),
            HourlyRate = (double)1.0m,
            DailyRate = (double)10.0m,
            MonthlyRate = (double)50.0m,
        };
        await Fixture.ParkingRateService.CreateParkingRateAsync(newParkingRate);

        // Act
        var dbParkingRate = await Fixture.ParkingRateService.GetParkingRateByIdAsync(newParkingRate.Id);
        var dbParkingRate2 = await Fixture.ParkingRateService.GetParkingRateByIdAsync(Guid.NewGuid().ToString());

        // Assert
        Assert.NotNull(dbParkingRate);
        Assert.Equal(newParkingRate.HourlyRate, dbParkingRate.HourlyRate);
        Assert.Equal(newParkingRate.DailyRate, dbParkingRate.DailyRate);
        Assert.Equal(newParkingRate.MonthlyRate, dbParkingRate.MonthlyRate);
        Assert.Equal(newParkingRate.Id, dbParkingRate.Id);
        Assert.NotNull(dbParkingRate.ConcurrencyStamp);
        Assert.True(Guid.TryParse(dbParkingRate.ConcurrencyStamp, out _), "ConcurrencyStamp should be a valid Guid.");

        Assert.Null(dbParkingRate2);

        // Clean up
        Fixture.Context.ParkingRates.RemoveRange(Fixture.Context.ParkingRates);
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

        var updatedParkingRate = new UpdateParkingRateDto<string>
        {
            Id = newParkingRate.Id,
            HourlyRate = (double)2.0m,
            DailyRate = (double)20.0m,
            MonthlyRate = (double)100.0m,
            ConcurrencyStamp = createdParkingRate?.ConcurrencyStamp ?? default!,
        };

        // Act
        var validationResult = await Fixture.ParkingRateService.UpdateParkingRateAsync(updatedParkingRate);
        var dbParkingRate = await Fixture.ParkingRateService.GetParkingRateByIdAsync(newParkingRate.Id);

        // Assert
        Assert.NotNull(validationResult);
        Assert.True(validationResult.Success, "ValidationResult should be successful.");

        Assert.NotNull(dbParkingRate);
        Assert.Equal(updatedParkingRate.HourlyRate, dbParkingRate.HourlyRate);
        Assert.Equal(updatedParkingRate.DailyRate, dbParkingRate.DailyRate);
        Assert.Equal(updatedParkingRate.MonthlyRate, dbParkingRate.MonthlyRate);
        Assert.Equal(newParkingRate.Id, dbParkingRate.Id);
        Assert.NotNull(dbParkingRate.ConcurrencyStamp);
        Assert.True(Guid.TryParse(dbParkingRate.ConcurrencyStamp, out _), "ConcurrencyStamp should be a valid Guid.");

        // Clean up
        Fixture.Context.ParkingRates.RemoveRange(Fixture.Context.ParkingRates);
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

        // Clean up
        Fixture.Context.ParkingRates.RemoveRange(Fixture.Context.ParkingRates);
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_ParkingRate()
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
        await Fixture.Context.SaveChangesAsync();

        // Act
        var deleteResult = await Fixture.ParkingRateService.DeleteParkingRateAsync(newParkingRate.Id);
        var deleteResult2 = await Fixture.ParkingRateService.DeleteParkingRateAsync(newParkingRate.Id);
        var dbParkingRate = await Fixture.ParkingRateService.GetParkingRateByIdAsync(newParkingRate.Id);

        // Assert
        Assert.True(deleteResult, "Delete result should be successful.");
        Assert.False(deleteResult2, "Delete result should not be successful.");

        Assert.Null(dbParkingRate);

        // Clean up
        Fixture.Context.ParkingRates.RemoveRange(Fixture.Context.ParkingRates);
    }
}
