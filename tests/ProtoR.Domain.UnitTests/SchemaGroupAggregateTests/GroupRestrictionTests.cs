namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests
{
    using System;
    using ProtoR.Domain.SchemaGroupAggregate;
    using Xunit;

    public class GroupRestrictionTests
    {
        [Fact]
        public void GroupRestriction_WithValidPattern_ShouldCreateGroupRestriction()
        {
            var restriction = new GroupRestriction("*Abc_01-1*");

            Assert.NotNull(restriction.Pattern);
        }

        [Fact]
        public void GroupRestriction_WithInvalidPattern_ShouldThrow()
        {
            Assert.Throws<ArgumentException>(() => new GroupRestriction("Ab$&cd*"));
        }

        [Fact]
        public void GroupRestriction_WithNulPattern_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new GroupRestriction(null));
        }

        [Fact]
        public void GroupRestriction_WithSamePattern_ShouldBeEqual()
        {
            var pattern = "*Abc_01-1*";
            var restrictionA = new GroupRestriction(pattern);
            var restrictionB = new GroupRestriction(pattern);

            Assert.True(restrictionA == restrictionB);
        }
    }
}
