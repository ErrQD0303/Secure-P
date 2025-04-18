namespace ApiTests.Fixtures;

[CollectionDefinition(nameof(ParkingZoneCollection), DisableParallelization = true)]
public class ParkingZoneCollection : ICollectionFixture<ParkingZoneFixture>
{
}