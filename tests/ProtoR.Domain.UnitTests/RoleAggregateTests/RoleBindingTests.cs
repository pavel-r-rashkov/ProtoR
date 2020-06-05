namespace ProtoR.Domain.UnitTests.RoleAggregateTests
{
    using System;
    using ProtoR.Domain.RoleAggregate;
    using Xunit;

    public class RoleBindingTests
    {
        [Fact]
        public void RoleBindings_WithEqualProperties_ShouldBeEqual()
        {
            var a = new RoleBinding(10, 10, null);
            var b = new RoleBinding(10, 10, null);

            Assert.True(a.Equals(b));
        }

        [Fact]
        public void RoleBinding_AssignedToNoOne_ShouldThrow()
        {
            Assert.Throws<ArgumentException>(() => new RoleBinding(10, null, null));
        }

        [Fact]
        public void RoleBinding_AssignedToUserAndClient_ShouldThrow()
        {
            Assert.Throws<ArgumentException>(() => new RoleBinding(10, 1, 2));
        }
    }
}
