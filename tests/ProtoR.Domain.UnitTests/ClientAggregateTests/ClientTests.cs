namespace ProtoR.Domain.UnitTests.ClientAggregateTests
{
    using System;
    using System.Linq;
    using ProtoR.Domain.CategoryAggregate;
    using ProtoR.Domain.ClientAggregate;
    using ProtoR.Domain.RoleAggregate;
    using Xunit;

    public class ClientTests
    {
        [Fact]
        public void SetRoles_ShouldSetRoleBindings()
        {
            var client = this.CreateClient();
            var roles = new long[] { 1, 2, 3 };

            client.SetRoles(roles);

            Assert.Equal(roles.Length, client.RoleBindings.Count());
            Assert.All(
                client.RoleBindings,
                roleBinding =>
                {
                    Assert.Contains(roleBinding.RoleId, roles);
                    Assert.Equal(client.Id, roleBinding.ClientId);
                });
        }

        [Fact]
        public void SetCategories_ShouldSetCategoryBindings()
        {
            var client = this.CreateClient();
            var categories = new long[] { 1, 2, 3 };

            client.SetCategories(categories);

            Assert.Equal(categories.Length, client.CategoryBindings.Count());
            Assert.All(
                client.CategoryBindings,
                categoryBinding =>
                {
                    Assert.Contains(categoryBinding.CategoryId, categories);
                    Assert.Equal(client.Id, categoryBinding.ClientId);
                });
        }

        [Fact]
        public void Client_ShouldSetProperties()
        {
            var client = this.CreateClient();

            Assert.NotNull(client.ClientId);
            Assert.NotNull(client.ClientName);
            Assert.NotNull(client.Secret);
            Assert.NotNull(client.GrantTypes);
            Assert.NotNull(client.RedirectUris);
            Assert.NotNull(client.PostLogoutRedirectUris);
            Assert.NotNull(client.AllowedCorsOrigins);
            Assert.NotNull(client.RoleBindings);
            Assert.NotNull(client.CategoryBindings);
        }

        private Client CreateClient()
        {
            var client = new Client(
                1,
                "Client ID",
                "Client Name",
                "secret",
                Array.Empty<string>(),
                Array.Empty<Uri>(),
                Array.Empty<Uri>(),
                Array.Empty<string>(),
                Array.Empty<RoleBinding>(),
                Array.Empty<CategoryBinding>());

            return client;
        }
    }
}
