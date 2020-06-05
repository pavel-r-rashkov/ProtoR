namespace ProtoR.Domain.UnitTests.RoleAggregateTests
{
    using System.Collections.Generic;
    using ProtoR.Domain.RoleAggregate;
    using Xunit;

    public class PermissionTests
    {
        [Fact]
        public void PermissionFromId_WithNonExistingId_ShouldThrow()
        {
            Assert.Throws<KeyNotFoundException>(() => Permission.FromId(999));
        }

        [Fact]
        public void PermissionFromId_WithExistingId_ShouldReturnPermission()
        {
            var id = 1;
            var permission = Permission.FromId(id);

            Assert.Equal(id, permission.Id);
        }
    }
}
