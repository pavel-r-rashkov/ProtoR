namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests.SchemasTests
{
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using Xunit;

    public class VersionTests
    {
        [Fact]
        public void Version_SouldBeCreated()
        {
            var version = new Version(1);

            Assert.NotNull(version);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(1, 30)]
        [InlineData(13, 14)]
        [InlineData(13, 23)]
        public void CompareTo_WithFirstVersionSmallerThanSecondOne_ShouldReturnNegativeValue(
            int versionA,
            int versionB)
        {
            var firstVersion = new Version(versionA);
            var secondVersion = new Version(versionB);

            int result = firstVersion.CompareTo(secondVersion);

            Assert.True(result < 0);
        }

        [Theory]
        [InlineData(2, 1)]
        [InlineData(30, 1)]
        [InlineData(14, 13)]
        [InlineData(23, 13)]
        public void CompareTo_WithFirstVersionGreaterThanSecondOne_ShouldReturnPositiveValue(
            int versionA,
            int versionB)
        {
            var firstVersion = new Version(versionA);
            var secondVersion = new Version(versionB);

            int result = firstVersion.CompareTo(secondVersion);

            Assert.True(result > 0);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 2)]
        [InlineData(463, 463)]
        public void CompareTo_WithEqualVersions_ShouldReturnZero(
            int versionA,
            int versionB)
        {
            var firstVersion = new Version(versionA);
            var secondVersion = new Version(versionB);

            int result = firstVersion.CompareTo(secondVersion);

            Assert.Equal(0, result);
        }

        [Fact]
        public void Initial_ShouldReturnFirstVersion()
        {
            var version = Version.Initial;

            Assert.Equal(1, version.VersionNumber);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        [InlineData(463, 464)]
        public void Next_ShouldReturnNextVersion(int versionNumber, int expected)
        {
            var version = new Version(versionNumber);

            Assert.Equal(expected, version.Next().VersionNumber);
        }

        [Fact]
        public void Equality_WithEqualProperties_ShouldBeTrue()
        {
            var firstVersion = new Version(1);
            var secondVersion = new Version(1);

            Assert.Equal(firstVersion, secondVersion);
        }

        [Fact]
        public void Equality_WithEqualVersions_ShouldBeTrue()
        {
            var firstVersion = new Version(1);
            var secondVersion = new Version(1);

            Assert.True(firstVersion == secondVersion);
        }

        [Fact]
        public void Inequality_WithDifferentVersions_ShouldBeTrue()
        {
            var firstVersion = new Version(1);
            var secondVersion = new Version(2);

            Assert.True(firstVersion != secondVersion);
        }

        [Fact]
        public void LessThan_WithFirstVersionSmallerThanSecondOne_ShouldBeTrue()
        {
            var firstVersion = new Version(1);
            var secondVersion = new Version(2);

            Assert.True(firstVersion < secondVersion);
        }

        [Fact]
        public void GreaterThan_WithFirstVersionGreaterThanSecondOne_ShouldBeTrue()
        {
            var firstVersion = new Version(2);
            var secondVersion = new Version(1);

            Assert.True(firstVersion > secondVersion);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        public void LessThanOrEqual_WithFirstVersionSmallerThanOrEqualToSecondOne_ShouldBeTrue(
            int firstVersionNumber,
            int secondVersionNumber)
        {
            var firstVersion = new Version(firstVersionNumber);
            var secondVersion = new Version(secondVersionNumber);

            Assert.True(firstVersion <= secondVersion);
        }

        [Theory]
        [InlineData(2, 2)]
        [InlineData(2, 1)]
        public void GreaterThan_WithFirstVersionGreaterThanOrEqualToSecondOne_ShouldBeTrue(
            int firstVersionNumber,
            int secondVersionNumber)
        {
            var firstVersion = new Version(firstVersionNumber);
            var secondVersion = new Version(secondVersionNumber);

            Assert.True(firstVersion >= secondVersion);
        }

        [Fact]
        public void GetHashCode_WithSameVersions_ShouldEqualVersionNumberCode()
        {
            var versionNumber = 3;
            var firstVersion = new Version(versionNumber);

            Assert.Equal(versionNumber.GetHashCode(), firstVersion.GetHashCode());
        }
    }
}
