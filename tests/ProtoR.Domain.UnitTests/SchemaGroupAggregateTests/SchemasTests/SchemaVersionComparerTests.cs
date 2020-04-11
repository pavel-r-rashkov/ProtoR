namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests.SchemasTests
{
    using System;
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.UnitTests.SchemaFixtures;
    using Xunit;
    using Version = ProtoR.Domain.SchemaGroupAggregate.Schemas.Version;

    // TODO: Replace with schema factory
    public class SchemaVersionComparerTests
    {
        private readonly string validSchema = SchemaFixtureUtils.GetProtoBuf("ValidSchema");
        private readonly ProtoBufSchemaFactory factory = new ProtoBufSchemaFactory();

        [Fact]
        public void Compare_WithSchemasWithEqualVersions_ShouldReturnZero()
        {
            var comparer = new SchemaVersionComparer<FileDescriptorSet>();
            var firstSchema = this.factory.CreateNew(new Version(1), this.validSchema);
            var secondSchema = this.factory.CreateNew(new Version(1), this.validSchema);

            int result = comparer.Compare(firstSchema, secondSchema);

            Assert.Equal(0, result);
        }

        [Fact]
        public void Compare_WithFirstSchemaOlderThanTheSecondOne_ShouldReturnNegativeValue()
        {
            var comparer = new SchemaVersionComparer<FileDescriptorSet>();
            var firstSchema = this.factory.CreateNew(new Version(1), this.validSchema);
            var secondSchema = this.factory.CreateNew(new Version(2), this.validSchema);

            int result = comparer.Compare(firstSchema, secondSchema);

            Assert.True(result < 0);
        }

        [Fact]
        public void Compare_WithFirstSchemaNewerThanTheSecondOne_ShouldReturnPositiveValue()
        {
            var comparer = new SchemaVersionComparer<FileDescriptorSet>();
            var firstSchema = this.factory.CreateNew(new Version(2), this.validSchema);
            var secondSchema = this.factory.CreateNew(new Version(1), this.validSchema);

            int result = comparer.Compare(firstSchema, secondSchema);

            Assert.True(result > 0);
        }

        [Fact]
        public void Compare_WithFirstSchemaNull_ShouldThrow()
        {
            var comparer = new SchemaVersionComparer<FileDescriptorSet>();
            var secondSchema = this.factory.CreateNew(new Version(1), this.validSchema);

            Assert.Throws<ArgumentNullException>(() => comparer.Compare(null, secondSchema));
        }

        [Fact]
        public void Compare_WithSecondSchemaNull_ShouldThrow()
        {
            var comparer = new SchemaVersionComparer<FileDescriptorSet>();
            var firstSchema = this.factory.CreateNew(new Version(1), this.validSchema);

            Assert.Throws<ArgumentNullException>(() => comparer.Compare(firstSchema, null));
        }
    }
}
