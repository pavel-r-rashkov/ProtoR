namespace ProtoR.DataAccess.IntegrationTests.Fixtures
{
    using Xunit;

    [CollectionDefinition(CollectionNames.IgniteCollection)]
    public class IgniteCollection : ICollectionFixture<IgniteFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition]
    }
}
