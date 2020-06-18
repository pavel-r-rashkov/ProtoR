namespace ProtoR.DataAccess.IntegrationTests.DataProviders
{
    using System;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using AutoFixture;
    using ProtoR.DataAccess.IntegrationTests.Fixtures;
    using ProtoR.Infrastructure.DataAccess.CacheItems;
    using ProtoR.Infrastructure.DataAccess.DataProviders;
    using Xunit;

    [Collection(CollectionNames.IgniteCollection)]
    public sealed class UserDataProviderTests : IDisposable
    {
        private readonly IgniteFixture igniteFixture;
        private readonly UserDataProvider dataProvider;
        private readonly ICache<long, UserCacheItem> userCache;
        private readonly ICache<UserRoleKey, EmptyCacheItem> userRoleCache;
        private readonly Fixture fixture;

        public UserDataProviderTests(IgniteFixture igniteFixture)
        {
            this.igniteFixture = igniteFixture;
            this.dataProvider = new UserDataProvider(this.igniteFixture.IgniteFactory, this.igniteFixture.Configuration);

            this.userCache = this.igniteFixture.IgniteFactory
                .Instance()
                .GetCache<long, UserCacheItem>(this.igniteFixture.Configuration.Value.UserCacheName);

            this.userRoleCache = this.igniteFixture.IgniteFactory
                .Instance()
                .GetCache<UserRoleKey, EmptyCacheItem>(this.igniteFixture.Configuration.Value.UserRoleCacheName);

            this.fixture = new Fixture();
            this.fixture.Customizations.Add(new UtcRandomDateTimeSequenceGenerator());
        }

        public void Dispose()
        {
            this.igniteFixture.CleanIgniteCache();
        }

        [Fact]
        public async Task GetById_ShouldReturnUser()
        {
            var id = 1;
            await this.InsertUser(id);

            var user = await this.dataProvider.GetById(id);

            Assert.NotNull(user);
        }

        [Fact]
        public async Task GetUsers_ShouldReturnUsers()
        {
            var usersCount = 3;

            for (int i = 0; i < usersCount; i++)
            {
                await this.InsertUser(i + 1);
            }

            var users = await this.dataProvider.GetUsers();

            Assert.NotEmpty(users);
        }

        private async Task InsertUser(long id)
        {
            var user = this.fixture.Create<UserCacheItem>();
            user.GroupRestrictions = "Abc*, B*";
            await this.userCache.PutAsync(id, user);

            await this.userRoleCache.PutAsync(
                new UserRoleKey { UserId = id, RoleId = 1 },
                new EmptyCacheItem());
        }
    }
}
