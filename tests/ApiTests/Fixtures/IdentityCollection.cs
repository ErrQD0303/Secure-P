namespace ApiTests.Fixtures;

[CollectionDefinition(nameof(IdentityCollection), DisableParallelization = true)]
public class IdentityCollection : ICollectionFixture<IdentityFixture>
{

}
