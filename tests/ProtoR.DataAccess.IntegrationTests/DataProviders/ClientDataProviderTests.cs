namespace ProtoR.DataAccess.IntegrationTests.DataProviders
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using AutoFixture;
    using ProtoR.DataAccess.IntegrationTests.Fixtures;
    using ProtoR.Infrastructure.DataAccess.CacheItems;
    using ProtoR.Infrastructure.DataAccess.DataProviders;
    using Xunit;

    [Collection(CollectionNames.IgniteCollection)]
    public sealed class ClientDataProviderTests : IDisposable
    {
        private readonly IgniteFixture igniteFixture;
        private readonly ClientDataProvider dataProvider;
        private readonly ICache<long, ClientCacheItem> clientCache;
        private readonly ICache<ClientRoleKey, EmptyCacheItem> clientRoleCache;
        private readonly Fixture fixture;

        public ClientDataProviderTests(IgniteFixture igniteFixture)
        {
            this.igniteFixture = igniteFixture;
            this.dataProvider = new ClientDataProvider(this.igniteFixture.IgniteFactory, this.igniteFixture.Configuration);

            this.clientCache = this.igniteFixture.IgniteFactory
                .Instance()
                .GetCache<long, ClientCacheItem>(this.igniteFixture.Configuration.Value.ClientCacheName);

            this.clientRoleCache = this.igniteFixture.IgniteFactory
                .Instance()
                .GetCache<ClientRoleKey, EmptyCacheItem>(this.igniteFixture.Configuration.Value.ClientRoleCacheName);

            this.fixture = new Fixture();
            this.fixture.Customizations.Add(new UtcRandomDateTimeSequenceGenerator());
        }

        public void Dispose()
        {
            this.igniteFixture.CleanIgniteCache();
        }

        [Fact]
        public async Task GetById_ShouldReturnClient()
        {
            var id = 1;
            await this.InsertClient(id);

            var client = await this.dataProvider.GetById(id);

            Assert.NotNull(client);
        }

        [Fact]
        public async Task GetClients_ShouldReturnClients()
        {
            var clientsCount = 3;

            for (int i = 0; i < clientsCount; i++)
            {
                await this.InsertClient(i + 1);
            }

            var clients = await this.dataProvider.GetClients();

            Assert.Equal(clientsCount, clients.Count());
        }

        [Fact]
        public async Task GetOrigins_ShouldReturnAllOrigins()
        {
            var clientsCount = 3;

            for (int i = 0; i < clientsCount; i++)
            {
                await this.InsertClient(i + 1);
            }

            var origins = await this.dataProvider.GetOrigins();

            Assert.NotEmpty(origins);
        }

        private async Task InsertClient(long id)
        {
            var client = this.fixture.Create<ClientCacheItem>();
            client.RedirectUris = string.Join(',', this.fixture.CreateMany<string>());
            client.PostLogoutRedirectUris = string.Join(',', this.fixture.CreateMany<string>());
            client.AllowedCorsOrigins = string.Join(',', this.fixture.CreateMany<string>());
            client.GroupRestrictions = "Abc*,B*";

            await this.clientCache.PutAsync(id, client);

            await this.clientRoleCache.PutAsync(
                new ClientRoleKey { ClientId = id, RoleId = 1 },
                new EmptyCacheItem());
        }
    }
}
