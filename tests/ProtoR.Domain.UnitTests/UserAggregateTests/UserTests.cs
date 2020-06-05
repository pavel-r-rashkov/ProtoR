namespace ProtoR.Domain.UnitTests.UserAggregateTests
{
    using System;
    using System.Linq;
    using ProtoR.Domain.CategoryAggregate;
    using ProtoR.Domain.RoleAggregate;
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
        public void SetCategories_ShouldSetCategoryBindings()
        {
            var user = this.CreateUser();
            var categories = new long[] { 1, 2, 3 };

            user.SetCategories(categories);

            Assert.Equal(categories.Length, user.CategoryBindings.Count());
            Assert.All(
                user.CategoryBindings,
                categoryBinding =>
                {
                    Assert.Contains(categoryBinding.CategoryId, categories);
                    Assert.Equal(user.Id, categoryBinding.UserId);
                });
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
                Array.Empty<RoleBinding>(),
                Array.Empty<CategoryBinding>());
        }
    }
}
