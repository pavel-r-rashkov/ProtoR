namespace ProtoR.Domain.UnitTests.ClientAggregateTests
{
    using System;
    using System.Linq;
    using ProtoR.Domain.ClientAggregate;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
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
        public void SetGroupRestrictions_ShouldSetGroupRestrictions()
        {
            var client = this.CreateClient();
            var restrictions = new GroupRestriction[]
            {
                new GroupRestriction("A*"),
                new GroupRestriction("B*"),
            };

            client.GroupRestrictions = restrictions;

            Assert.Equal(restrictions.Length, client.GroupRestrictions.Count());
        }

        [Fact]
        public void SetGroupRestrictions_WithNullGroupRestrictions_ShouldThrow()
        {
            var client = this.CreateClient();
            GroupRestriction[] restrictions = null;

            Assert.Throws<ArgumentNullException>(() => client.GroupRestrictions = restrictions);
        }

        [Fact]
        public void SetGroupRestrictions_WithEmptyGroupRestrictions_ShouldThrow()
        {
            var client = this.CreateClient();
            GroupRestriction[] restrictions = Array.Empty<GroupRestriction>();

            Assert.Throws<ArgumentException>(() => client.GroupRestrictions = restrictions);
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
            Assert.NotNull(client.GroupRestrictions);
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
                new GroupRestriction[] { new GroupRestriction("*") },
                Array.Empty<RoleBinding>());

            return client;
        }
    }
}
