namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests.SchemasTests
{
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.UnitTests.SchemaFixtures;
    using Xunit;

    public class ProtoBufSchemaFactoryTests
    {
        [Fact]
        public void CreateNew_WithInvalidSchemaContents_ShouldThrow()
        {
            var factory = new ProtoBufSchemaFactory();
            var contents = "invalid protobuf schemacontents";

            Assert.Throws<InvalidProtoBufSchemaException>(() => factory.CreateNew(new Version(1), contents));
        }

        [Fact]
        public void CreateNew_WithValidSchemaContents_ShouldNewSchema()
        {
            var factory = new ProtoBufSchemaFactory();
            var contents = SchemaFixtureUtils.GetProtoBuf("ValidSchema");

            Schema<FileDescriptorSet> schema = factory.CreateNew(new Version(1), contents);

            Assert.NotNull(schema);
        }
    }
}
