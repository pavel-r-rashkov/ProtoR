namespace ProtoR.Domain.UnitTests.RoleAggregateTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ProtoR.Domain.RoleAggregate;
    using Xunit;

    public class RoleTests
    {
        [Fact]
        public void Role_WithNullName_ShouldThrow()
        {
            Assert.Throws<ArgumentException>(() => new Role(1, null, "name", Array.Empty<Permission>()));
        }

        [Fact]
        public void Role_WithEmptyName_ShouldThrow()
        {
            Assert.Throws<ArgumentException>(() => new Role(1, string.Empty, "name", Array.Empty<Permission>()));
        }

        [Fact]
        public void Role_WithNullNormalizedName_ShouldThrow()
        {
            Assert.Throws<ArgumentException>(() => new Role(1, "name", null, Array.Empty<Permission>()));
        }

        [Fact]
        public void Role_WithEmptyNormalizedName_ShouldThrow()
        {
            Assert.Throws<ArgumentException>(() => new Role(1, "name", string.Empty, Array.Empty<Permission>()));
        }

        [Fact]
        public void Role_ShouldSetProperties()
        {
            var role = new Role(1, "name", "NAME", Array.Empty<Permission>());

            Assert.NotNull(role.Name);
            Assert.NotNull(role.NormalizedName);
        }

        [Fact]
        public void Role_ShouldSetPermissionsToEmptyCollection()
        {
            var role = new Role(1, "name", "NAME", null);

            Assert.NotNull(role.Permissions);
        }

        [Fact]
        public void Role_AssignPermissions_ShouldSetPermissions()
        {
            var role = new Role(1, "name", "NAME", Array.Empty<Permission>());
            var permissions = new List<Permission>
            {
                Permission.RoleRead,
                Permission.RoleWrite,
            };

            role.AssignPermissions(permissions);

            Assert.Equal(permissions.Count, role.Permissions.Count());
        }
    }
}
