namespace ProtoR.DataAccess.IntegrationTests.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Linq;
    using ProtoR.DataAccess.IntegrationTests.Fixtures;
    using ProtoR.Domain.CategoryAggregate;
    using ProtoR.Domain.ClientAggregate;
    using ProtoR.Domain.RoleAggregate;
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
        private readonly ICache<ClientCategoryKey, EmptyCacheItem> clientCategoryCache;

        public ClientRepositoryTests(IgniteFixture igniteFixture)
        {
            this.igniteFixture = igniteFixture;

            var userProviderStub = new UserProviderStub();
            this.repository = new ClientRepository(
                this.igniteFixture.IgniteFactory,
                this.igniteFixture.Configuration,
                userProviderStub);

            this.clientCache = this.igniteFixture.IgniteFactory.Instance().GetCache<long, ClientCacheItem>(this.igniteFixture.Configuration.ClientCacheName);
            this.clientRoleCache = this.igniteFixture.IgniteFactory.Instance().GetCache<ClientRoleKey, EmptyCacheItem>(this.igniteFixture.Configuration.ClientRoleCacheName);
            this.clientCategoryCache = this.igniteFixture.IgniteFactory.Instance().GetCache<ClientCategoryKey, EmptyCacheItem>(this.igniteFixture.Configuration.ClientCategoryCacheName);
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
            var grantTypes = new List<string> { "code" };
            var testUri = "http://test.com/";
            var redirectUris = new List<Uri> { new Uri(testUri) };
            var postLogoutRedirectUris = new List<Uri> { new Uri(testUri) };
            var testOrigin = "http://test.com";
            var allowedCorsOrigins = new List<string> { testOrigin };
            var roleId = 1;
            var roleBindings = new List<RoleBinding> { new RoleBinding(roleId, null, defaultId) };
            var categoryId = 1;
            var categoryBindings = new List<CategoryBinding> { new CategoryBinding(categoryId, null, defaultId) };

            var client = new Client(
                defaultId,
                clientId,
                clientName,
                secret,
                grantTypes,
                redirectUris,
                postLogoutRedirectUris,
                allowedCorsOrigins,
                roleBindings,
                categoryBindings);

            var id = await this.repository.Add(client);

            var clientCacheItem = await this.clientCache.GetAsync(id);
            Assert.NotNull(clientCacheItem);
            Assert.Equal(clientId, clientCacheItem.ClientId);
            Assert.Equal(clientName, clientCacheItem.ClientName);
            Assert.Equal(secret, clientCacheItem.Secret);
            Assert.Contains(testUri, clientCacheItem.RedirectUris, StringComparison.InvariantCultureIgnoreCase);
            Assert.Contains(testUri, clientCacheItem.PostLogoutRedirectUris, StringComparison.InvariantCultureIgnoreCase);
            Assert.Contains(testOrigin, clientCacheItem.AllowedCorsOrigins, StringComparison.InvariantCultureIgnoreCase);

            var roleBinding = this.clientRoleCache
                .AsCacheQueryable()
                .Where(r => r.Key.ClientId == id)
                .Select(r => r.Key)
                .FirstOrDefault();

            Assert.NotNull(roleBinding);
            Assert.Equal(roleId, roleBinding.RoleId);
            Assert.Equal(id, roleBinding.ClientId);

            var categoryBinding = this.clientCategoryCache
                .AsCacheQueryable()
                .Where(r => r.Key.ClientId == id)
                .Select(r => r.Key)
                .FirstOrDefault();

            Assert.NotNull(categoryBinding);
            Assert.Equal(categoryId, categoryBinding.CategoryId);
            Assert.Equal(id, categoryBinding.ClientId);
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

            var categoryBindings = this.clientCategoryCache
                .AsCacheQueryable()
                .Where(r => r.Key.ClientId == client.Id)
                .Select(r => r.Key)
                .ToList();

            Assert.False(clientCacheItem.Success);
            Assert.Empty(roleBindings);
            Assert.Empty(categoryBindings);
        }

        [Fact]
        public async Task GetByClientId_ShouldReturnClient()
        {
            var insertedClient = await this.InsertClient();

            var client = await this.repository.GetByClientId(insertedClient.ClientId);

            Assert.NotNull(client);
            Assert.NotNull(client.ClientName);
            Assert.NotNull(client.Secret);
            Assert.NotEmpty(client.GrantTypes);
            Assert.NotEmpty(client.RedirectUris);
            Assert.NotEmpty(client.PostLogoutRedirectUris);
            Assert.NotEmpty(client.AllowedCorsOrigins);
            Assert.NotEmpty(client.RoleBindings);
            Assert.NotEmpty(client.CategoryBindings);
        }

        [Fact]
        public async Task GetById_ShouldReturnClient()
        {
            var insertedClient = await this.InsertClient();

            var client = await this.repository.GetById(insertedClient.Id);

            Assert.NotNull(client);
            Assert.NotNull(client.ClientName);
            Assert.NotNull(client.Secret);
            Assert.NotEmpty(client.GrantTypes);
            Assert.NotEmpty(client.RedirectUris);
            Assert.NotEmpty(client.PostLogoutRedirectUris);
            Assert.NotEmpty(client.AllowedCorsOrigins);
            Assert.NotEmpty(client.RoleBindings);
            Assert.NotEmpty(client.CategoryBindings);
        }

        [Fact]
        public async Task Update_ShouldUpdateClient()
        {
            var client = await this.InsertClient();
            var newRoleId = 1;
            var newCategoryId = 1;
            var newClientId = "updated client id";
            var newClientName = "updated client name";
            var newSecret = "updated secret";
            var newGrantType = "client_credentials";
            var newRedirectUri = "https://updatedredirect.com/";
            var newPostLogoutUri = "https://updatedpostlogout.com/";
            var newOrigin = "https://updated.com";

            client.ClientId = newClientId;
            client.ClientName = newClientName;
            client.Secret = newSecret;
            client.GrantTypes = new List<string> { newGrantType };
            client.RedirectUris = new List<Uri> { new Uri(newRedirectUri) };
            client.PostLogoutRedirectUris = new List<Uri> { new Uri(newPostLogoutUri) };
            client.AllowedCorsOrigins = new List<string> { newOrigin };
            client.SetRoles(new long[] { newRoleId });
            client.SetCategories(new long[] { newCategoryId });

            await this.repository.Update(client);

            var clientCacheItem = await this.clientCache.GetAsync(client.Id);

            var roleBindings = this.clientRoleCache
                .AsCacheQueryable()
                .Where(r => r.Key.ClientId == client.Id)
                .Select(r => r.Key)
                .ToList();

            var categoryBindings = this.clientCategoryCache
                .AsCacheQueryable()
                .Where(r => r.Key.ClientId == client.Id)
                .Select(r => r.Key)
                .ToList();

            Assert.Equal(newClientId, clientCacheItem.ClientId);
            Assert.Equal(newClientName, clientCacheItem.ClientName);
            Assert.Equal(newSecret, clientCacheItem.Secret);
            Assert.Equal(newGrantType, clientCacheItem.GrantTypes);
            Assert.Equal(newRedirectUri, clientCacheItem.RedirectUris);
            Assert.Equal(newPostLogoutUri, clientCacheItem.PostLogoutRedirectUris);
            Assert.Equal(newOrigin, clientCacheItem.AllowedCorsOrigins);

            Assert.Single(roleBindings);
            Assert.Equal(newRoleId, roleBindings.First().RoleId);

            Assert.Single(categoryBindings);
            Assert.Equal(newCategoryId, categoryBindings.First().CategoryId);
        }

        private async Task<Client> InsertClient()
        {
            var id = 1;
            var clientId = "client id";
            var clientName = "client name";
            var secret = "abc123";
            var grantTypes = new List<string> { "code" };
            var testUri = "http://test.com/";
            var redirectUris = new List<Uri> { new Uri(testUri) };
            var postLogoutRedirectUris = new List<Uri> { new Uri(testUri) };
            var testOrigin = "http://test.com";
            var allowedCorsOrigins = new List<string> { testOrigin };
            var roleId = 1;
            var roleBindings = new List<RoleBinding> { new RoleBinding(roleId, null, id) };
            var categoryId = 1;
            var categoryBindings = new List<CategoryBinding> { new CategoryBinding(categoryId, null, id) };

            var client = new Client(
                id,
                clientId,
                clientName,
                secret,
                grantTypes,
                redirectUris,
                postLogoutRedirectUris,
                allowedCorsOrigins,
                roleBindings,
                categoryBindings);

            await this.clientCache.PutAsync(id, new ClientCacheItem
            {
                ClientId = clientId,
                ClientName = clientName,
                Secret = secret,
                GrantTypes = testUri,
                RedirectUris = testUri,
                PostLogoutRedirectUris = testUri,
                AllowedCorsOrigins = testOrigin,
                CreatedBy = "test user",
                CreatedOn = DateTime.UtcNow,
            });

            await this.clientRoleCache.PutAsync(
                new ClientRoleKey { ClientId = id, RoleId = roleId },
                new EmptyCacheItem());

            await this.clientCategoryCache.PutAsync(
                new ClientCategoryKey { ClientId = id, CategoryId = categoryId },
                new EmptyCacheItem());

            return client;
        }
    }
}
