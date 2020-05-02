namespace ProtoR.ComponentTests.Configuration
{
    using Xunit;

    [CollectionDefinition(CollectionNames.TestApplicationCollection)]
    public class TestApplicationCollection : ICollectionFixture<ComponentTestApplicationFactory>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition]
    }
}
