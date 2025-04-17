namespace ApiTests.Fixtures;

[CollectionDefinition(nameof(ParkingRateCollection), DisableParallelization = true)]
public class ParkingRateCollection : ICollectionFixture<ParkingRateFixture>
{
}