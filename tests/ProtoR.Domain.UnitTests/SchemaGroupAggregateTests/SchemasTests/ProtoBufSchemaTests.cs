namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests.SchemasTests
{
    using System;
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.UnitTests.SchemaFixtures;
    using Xunit;

    public class ProtoBufSchemaTests
    {
        [Fact]
        public void Parsed_ShouldReturnParsedSchemaContents()
        {
            string contents = SchemaFixtureUtils.GetProtoBuf("ValidSchema");
            var schema = new ProtoBufSchema(
                1,
                new SchemaGroupAggregate.Schemas.Version(1),
                contents);

            FileDescriptorSet descriptor = schema.Parsed;

            Assert.NotNull(descriptor);
            Assert.Empty(descriptor.GetErrors());
        }
    }
}
