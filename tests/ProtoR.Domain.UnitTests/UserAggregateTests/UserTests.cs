namespace ProtoR.Domain.UnitTests.UserAggregateTests
{
    using System;
    using System.Linq;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.UserAggregate;
    using Xunit;

    public class UserTests
    {
        [Fact]
        public void SetRoles_ShouldSetRoleBindings()
        {
            var user = this.CreateUser();
            var roles = new long[] { 1, 2, 3 };

            user.SetRoles(roles);

            Assert.Equal(roles.Length, user.RoleBindings.Count());
            Assert.All(
                user.RoleBindings,
                roleBinding =>
                {
                    Assert.Contains(roleBinding.RoleId, roles);
                    Assert.Equal(user.Id, roleBinding.UserId);
                });
        }

        [Fact]
        public void AddRole_WithNonExistingRole_ShouldAddRole()
        {
            var user = this.CreateUser();
            var roles = new long[] { 1, 2, 3 };
            user.SetRoles(roles);
            var newRole = 4;

            user.AddRole(newRole);

            Assert.Equal(4, user.RoleBindings.Count());
            Assert.Contains(user.RoleBindings, roleBinding => roleBinding.RoleId == newRole);
        }

        [Fact]
        public void AddRole_WithExistingRole_ShouldNotAddDuplicatedRole()
        {
            var user = this.CreateUser();
            var roles = new long[] { 1, 2, 3 };
            user.SetRoles(roles);
            var newRole = 3;

            user.AddRole(newRole);

            Assert.Equal(3, user.RoleBindings.Count());
        }

        [Fact]
        public void RemoveRole_ShouldRemoveRole()
        {
            var user = this.CreateUser();
            var roles = new long[] { 1, 2, 3 };
            user.SetRoles(roles);
            var roleToRemove = 3;

            user.RemoveRole(roleToRemove);

            Assert.Equal(2, user.RoleBindings.Count());
        }

        [Fact]
        public void SetGroupRestrictions_ShouldSetGroupRestrictions()
        {
            var user = this.CreateUser();
            var restrictions = new GroupRestriction[]
            {
                new GroupRestriction("A*"),
                new GroupRestriction("B*"),
            };

            user.GroupRestrictions = restrictions;

            Assert.Equal(restrictions.Length, user.GroupRestrictions.Count());
        }

        [Fact]
        public void SetGroupRestrictions_WithNullGroupRestrictions_ShouldThrow()
        {
            var user = this.CreateUser();
            GroupRestriction[] restrictions = null;

            Assert.Throws<ArgumentNullException>(() => user.GroupRestrictions = restrictions);
        }

        [Fact]
        public void SetGroupRestrictions_WithEmptyGroupRestrictions_ShouldThrow()
        {
            var user = this.CreateUser();
            GroupRestriction[] restrictions = Array.Empty<GroupRestriction>();

            Assert.Throws<ArgumentException>(() => user.GroupRestrictions = restrictions);
        }

        [Fact]
        public void User_ShouldSetProperties()
        {
            var user = this.CreateUser();

            Assert.NotNull(user.UserName);
            Assert.NotNull(user.NormalizedUserName);
            Assert.NotNull(user.PasswordHash);
        }

        [Fact]
        public void User_ShouldSetUserName()
        {
            var user = new User("name");

            Assert.NotNull(user.UserName);
        }

        private User CreateUser()
        {
            return new User(
                1,
                "test user",
                "TEST USER",
                "abc123",
                true,
                new GroupRestriction[] { new GroupRestriction("*") },
                Array.Empty<RoleBinding>());
        }
    }
}
