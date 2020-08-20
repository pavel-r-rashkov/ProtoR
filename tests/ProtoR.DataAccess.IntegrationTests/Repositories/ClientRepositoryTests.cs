namespace ProtoR.DataAccess.IntegrationTests.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Linq;
    using ProtoR.DataAccess.IntegrationTests.Fixtures;
    using ProtoR.Domain.ClientAggregate;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Infrastructure.DataAccess;
    using ProtoR.Infrastructure.DataAccess.CacheItems;
    using ProtoR.Infrastructure.DataAccess.Repositories;
    using Xunit;

    [Collection(CollectionNames.IgniteCollection)]
    public sealed class ClientRepositoryTests : IDisposable
    {
        private readonly IgniteFixture igniteFixture;
        private readonly ClientRepository repository;
        private readonly ICache<long, ClientCacheItem> clientCache;
        private readonly ICache<ClientRoleKey, EmptyCacheItem> clientRoleCache;
        private readonly ICache<long, RoleCacheItem> roleCache;

        public ClientRepositoryTests(IgniteFixture igniteFixture)
        {
            this.igniteFixture = igniteFixture;

            var userProviderStub = new UserProviderStub();
            this.repository = new ClientRepository(
                this.igniteFixture.IgniteFactory,
                this.igniteFixture.Configuration,
                userProviderStub);

            var cacheNames = this.igniteFixture.Configuration.Value.CacheNames;

            this.clientCache = this.igniteFixture.IgniteFactory
                .Instance()
                .GetCache<long, ClientCacheItem>(cacheNames.ClientCacheName);

            this.clientRoleCache = this.igniteFixture.IgniteFactory
                .Instance()
                .GetCache<ClientRoleKey, EmptyCacheItem>(cacheNames.ClientRoleCacheName);

            this.roleCache = this.igniteFixture.IgniteFactory
                .Instance()
                .GetCache<long, RoleCacheItem>(cacheNames.RoleCacheName);
        }

        public void Dispose()
        {
            this.igniteFixture.CleanIgniteCache();
        }

        [Fact]
        public async Task Add_ShouldInsertClient()
        {
            long defaultId = default;
            var clientId = "client id";
            var clientName = "client name";
            var secret = "abc123";
            var isActive = true;
            var grantTypes = new List<string> { "code" };
            var testUri = "http://test.com/";
            var redirectUris = new List<Uri> { new Uri(testUri) };
            var postLogoutRedirectUris = new List<Uri> { new Uri(testUri) };
            var testOrigin = "http://test.com";
            var allowedCorsOrigins = new List<string> { testOrigin };
            var roleId = 1;
            var roleBindings = new List<RoleBinding> { new RoleBinding(roleId, null, defaultId) };
            var pattern = "Abc*";
            var groupRestrictions = new List<GroupRestriction> { new GroupRestriction(pattern) };

            var client = new Client(
                defaultId,
                clientId,
                clientName,
                secret,
                isActive,
                grantTypes,
                redirectUris,
                postLogoutRedirectUris,
                allowedCorsOrigins,
                groupRestrictions,
                roleBindings);

            await this.roleCache.PutAsync(roleId, new RoleCacheItem
            {
                Name = "testrole",
                NormalizedName = "TESTROLE",
                CreatedBy = "Author",
                CreatedOn = DateTime.UtcNow,
            });

            var id = await this.repository.Add(client);

            var clientCacheItem = await this.clientCache.GetAsync(id);
            Assert.NotNull(clientCacheItem);
            Assert.Equal(clientId, clientCacheItem.ClientId);
            Assert.Equal(clientName, clientCacheItem.ClientName);
            Assert.Equal(secret, clientCacheItem.Secret);
            Assert.Equal(isActive, clientCacheItem.IsActive);
            Assert.Contains(testUri, clientCacheItem.RedirectUris, StringComparison.InvariantCultureIgnoreCase);
            Assert.Contains(testUri, clientCacheItem.PostLogoutRedirectUris, StringComparison.InvariantCultureIgnoreCase);
            Assert.Contains(testOrigin, clientCacheItem.AllowedCorsOrigins, StringComparison.InvariantCultureIgnoreCase);
            Assert.Contains(pattern, clientCacheItem.GroupRestrictions, StringComparison.InvariantCultureIgnoreCase);

            var roleBinding = this.clientRoleCache
                .AsCacheQueryable()
                .Where(r => r.Key.ClientId == id)
                .Select(r => r.Key)
                .FirstOrDefault();

            Assert.NotNull(roleBinding);
            Assert.Equal(roleId, roleBinding.RoleId);
            Assert.Equal(id, roleBinding.ClientId);
        }

        [Fact]
        public async Task Add_WithNonExistingRole_ShouldThrow()
        {
            var client = new Client(
                default,
                "client id",
                "client name",
                "abc123",
                true,
                new List<string> { "code" },
                new List<Uri> { new Uri("http://test.com/") },
                new List<Uri> { new Uri("http://test.com/") },
                new List<string> { "http://test.com" },
                new List<GroupRestriction> { new GroupRestriction("*") },
                new List<RoleBinding> { new RoleBinding(10, null, default(long)) });

            await Assert.ThrowsAsync<ForeignKeyViolationException>(async () => await this.repository.Add(client));
        }

        [Fact]
        public async Task Delete_ShouldRemoveClient()
        {
            var client = await this.InsertClient();

            await this.repository.Delete(client.Id);

            var clientCacheItem = await this.clientCache.TryGetAsync(client.Id);
            var roleBindings = this.clientRoleCache
                .AsCacheQueryable()
                .Where(r => r.Key.ClientId == client.Id)
                .Select(r => r.Key)
                .ToList();
        }

        [Fact]
        public async Task GetByClientId_ShouldReturnClient()
        {
            var insertedClient = await this.InsertClient();

            var client = await this.repository.GetByClientId(insertedClient.ClientId);

            Assert.NotNull(client);
            Assert.NotNull(client.ClientName);
            Assert.NotNull(client.Secret);
            Assert.True(client.IsActive);
            Assert.NotEmpty(client.GrantTypes);
            Assert.NotEmpty(client.RedirectUris);
            Assert.NotEmpty(client.PostLogoutRedirectUris);
            Assert.NotEmpty(client.AllowedCorsOrigins);
            Assert.NotEmpty(client.RoleBindings);
            Assert.NotEmpty(client.GroupRestrictions);
        }

        [Fact]
        public async Task GetById_ShouldReturnClient()
        {
            var insertedClient = await this.InsertClient();

            var client = await this.repository.GetById(insertedClient.Id);

            Assert.NotNull(client);
            Assert.NotNull(client.ClientName);
            Assert.NotNull(client.Secret);
            Assert.True(client.IsActive);
            Assert.NotEmpty(client.GrantTypes);
            Assert.NotEmpty(client.RedirectUris);
            Assert.NotEmpty(client.PostLogoutRedirectUris);
            Assert.NotEmpty(client.AllowedCorsOrigins);
            Assert.NotEmpty(client.RoleBindings);
            Assert.NotEmpty(client.GroupRestrictions);
        }

        [Fact]
        public async Task Update_ShouldUpdateClient()
        {
            var client = await this.InsertClient();
            var newRoleId = 1;
            var newClientId = "updated client id";
            var newClientName = "updated client name";
            var newSecret = "updated secret";
            var newActiveState = false;
            var newGrantType = "client_credentials";
            var newRedirectUri = "https://updatedredirect.com/";
            var newPostLogoutUri = "https://updatedpostlogout.com/";
            var newOrigin = "https://updated.com";
            var newGroupRestriction = "Abc*";

            client.ClientId = newClientId;
            client.ClientName = newClientName;
            client.Secret = newSecret;
            client.IsActive = newActiveState;
            client.GrantTypes = new List<string> { newGrantType };
            client.RedirectUris = new List<Uri> { new Uri(newRedirectUri) };
            client.PostLogoutRedirectUris = new List<Uri> { new Uri(newPostLogoutUri) };
            client.AllowedCorsOrigins = new List<string> { newOrigin };
            client.GroupRestrictions = new GroupRestriction[] { new GroupRestriction(newGroupRestriction) };
            client.SetRoles(new long[] { newRoleId });

            await this.repository.Update(client);

            var clientCacheItem = await this.clientCache.GetAsync(client.Id);

            var roleBindings = this.clientRoleCache
                .AsCacheQueryable()
                .Where(r => r.Key.ClientId == client.Id)
                .Select(r => r.Key)
                .ToList();

            Assert.Equal(newClientId, clientCacheItem.ClientId);
            Assert.Equal(newClientName, clientCacheItem.ClientName);
            Assert.Equal(newSecret, clientCacheItem.Secret);
            Assert.Equal(newActiveState, clientCacheItem.IsActive);
            Assert.Equal(newGrantType, clientCacheItem.GrantTypes);
            Assert.Equal(newRedirectUri, clientCacheItem.RedirectUris);
            Assert.Equal(newPostLogoutUri, clientCacheItem.PostLogoutRedirectUris);
            Assert.Equal(newOrigin, clientCacheItem.AllowedCorsOrigins);
            Assert.Equal(newGroupRestriction, clientCacheItem.GroupRestrictions);

            Assert.Single(roleBindings);
            Assert.Equal(newRoleId, roleBindings.First().RoleId);
        }

        [Fact]
        public async Task Update_WithNonExistingRole_ShouldThrow()
        {
            var client = await this.InsertClient();
            var newRoleId = 10;
            client.SetRoles(new long[] { newRoleId });

            await Assert.ThrowsAsync<ForeignKeyViolationException>(async () => await this.repository.Update(client));
        }

        private async Task<Client> InsertClient()
        {
            var id = 1;
            var clientId = "client id";
            var clientName = "client name";
            var secret = "abc123";
            var isActive = true;
            var grantTypes = new List<string> { "code" };
            var testUri = "http://test.com/";
            var redirectUris = new List<Uri> { new Uri(testUri) };
            var postLogoutRedirectUris = new List<Uri> { new Uri(testUri) };
            var testOrigin = "http://test.com";
            var allowedCorsOrigins = new List<string> { testOrigin };
            var roleId = 1;
            var roleBindings = new List<RoleBinding> { new RoleBinding(roleId, null, id) };
            var groupRestrictions = new GroupRestriction[] { new GroupRestriction("*") };

            var client = new Client(
                id,
                clientId,
                clientName,
                secret,
                isActive,
                grantTypes,
                redirectUris,
                postLogoutRedirectUris,
                allowedCorsOrigins,
                groupRestrictions,
                roleBindings);

            await this.clientCache.PutAsync(id, new ClientCacheItem
            {
                ClientId = clientId,
                ClientName = clientName,
                Secret = secret,
                IsActive = isActive,
                GrantTypes = testUri,
                RedirectUris = testUri,
                PostLogoutRedirectUris = testUri,
                AllowedCorsOrigins = testOrigin,
                GroupRestrictions = groupRestrictions.First().Pattern,
                CreatedBy = "test user",
                CreatedOn = DateTime.UtcNow,
            });

            await this.clientRoleCache.PutAsync(
                new ClientRoleKey { ClientId = id, RoleId = roleId },
                new EmptyCacheItem());

            return client;
        }
    }
}
