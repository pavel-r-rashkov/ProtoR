namespace ProtoR.DataAccess.IntegrationTests.DataProviders
{
    using System;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using AutoFixture;
    using ProtoR.Application.Common;
    using ProtoR.DataAccess.IntegrationTests.Fixtures;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Infrastructure.DataAccess.CacheItems;
    using ProtoR.Infrastructure.DataAccess.DataProviders;
    using Xunit;

    [Collection(CollectionNames.IgniteCollection)]
    public sealed class RoleDataProviderTests : IDisposable
    {
        private readonly IgniteFixture igniteFixture;
        private readonly RoleDataProvider dataProvider;
        private readonly ICache<long, RoleCacheItem> roleCache;
        private readonly ICache<RolePermissionKey, EmptyCacheItem> rolePermissionCache;
        private readonly Fixture fixture;

        public RoleDataProviderTests(IgniteFixture igniteFixture)
        {
            this.igniteFixture = igniteFixture;
            this.dataProvider = new RoleDataProvider(this.igniteFixture.IgniteFactory, this.igniteFixture.Configuration);
            var cacheNames = this.igniteFixture.Configuration.Value.CacheNames;

            this.roleCache = this.igniteFixture.IgniteFactory
                .Instance()
                .GetCache<long, RoleCacheItem>(cacheNames.RoleCacheName);

            this.rolePermissionCache = this.igniteFixture.IgniteFactory
                .Instance()
                .GetCache<RolePermissionKey, EmptyCacheItem>(cacheNames.RolePermissionCacheName);

            this.fixture = new Fixture();
            this.fixture.Customizations.Add(new UtcRandomDateTimeSequenceGenerator());
        }

        public void Dispose()
        {
            this.igniteFixture.CleanIgniteCache();
        }

        [Fact]
        public async Task GetById_ShouldReturnRole()
        {
            var id = 1;
            await this.InsertRole(id);

            var role = await this.dataProvider.GetById(id);

            Assert.NotNull(role);
        }

        [Fact]
        public async Task GetRoles_ShouldReturnRoles()
        {
            var rolesCount = 3;

            for (int i = 0; i < rolesCount; i++)
            {
                await this.InsertRole(i + 1);
            }

            var roles = await this.dataProvider.GetRoles(
                Array.Empty<Filter>(),
                SortOrder.Default("Id"),
                Pagination.Default());

            Assert.NotEmpty(roles.Items);
        }

        private async Task InsertRole(int id)
        {
            var role = this.fixture.Create<RoleCacheItem>();
            await this.roleCache.PutAsync(id, role);

            await this.rolePermissionCache.PutAsync(
                new RolePermissionKey { RoleId = id, PermissionId = Permission.SchemaRead.Id },
                new EmptyCacheItem());
        }
    }
}
