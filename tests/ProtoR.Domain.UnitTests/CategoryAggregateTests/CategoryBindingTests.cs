namespace ProtoR.Domain.UnitTests.CategoryAggregateTests
{
    using System;
    using ProtoR.Domain.CategoryAggregate;
    using Xunit;

    public class CategoryBindingTests
    {
        [Fact]
        public void CategoryBindings_WithEqualProperties_ShouldBeEqual()
        {
            var a = new CategoryBinding(10, 10, null);
            var b = new CategoryBinding(10, 10, null);

            Assert.True(a.Equals(b));
        }

        [Fact]
        public void CategoryBinding_AssignedToNoOne_ShouldThrow()
        {
            Assert.Throws<ArgumentException>(() => new CategoryBinding(10, null, null));
        }

        [Fact]
        public void CategoryBinding_AssignedToUserAndClient_ShouldThrow()
        {
            Assert.Throws<ArgumentException>(() => new CategoryBinding(10, 1, 2));
        }
    }
}
