namespace ProtoR.DataAccess.IntegrationTests.DataProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using AutoFixture;
    using ProtoR.Application.Common;
    using ProtoR.Application.Group;
    using ProtoR.DataAccess.IntegrationTests.Fixtures;
    using ProtoR.Infrastructure.DataAccess.CacheItems;
    using ProtoR.Infrastructure.DataAccess.DataProviders;
    using Xunit;

    [Collection(CollectionNames.IgniteCollection)]
    public sealed class GroupDataProviderTests : IDisposable
    {
        private readonly IgniteFixture igniteFixture;
        private readonly GroupDataProvider dataProvider;
        private readonly ICache<long, SchemaGroupCacheItem> groupCache;
        private readonly Fixture fixture = new Fixture();

        public GroupDataProviderTests(IgniteFixture igniteFixture)
        {
            this.igniteFixture = igniteFixture;
            this.dataProvider = new GroupDataProvider(this.igniteFixture.IgniteFactory, this.igniteFixture.Configuration);

            this.groupCache = this.igniteFixture.IgniteFactory
                .Instance()
                .GetCache<long, SchemaGroupCacheItem>(this.igniteFixture.Configuration.Value.CacheNames.SchemaGroupCacheName);

            this.fixture.Customizations.Add(new UtcRandomDateTimeSequenceGenerator());
        }

        public void Dispose()
        {
            this.igniteFixture.CleanIgniteCache();
        }

        [Fact]
        public async Task GetByName_ShouldReturnGroupWithSpecifiedName()
        {
            var group = this.fixture.Create<SchemaGroupCacheItem>();
            group.CreatedOn = DateTime.UtcNow;
            this.groupCache.Put(1, group);

            GroupDto result = await this.dataProvider.GetByName(group.Name);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetGroups_ShouldReturnListOfGroups()
        {
            var groupsCount = 5;

            for (int i = 1; i <= groupsCount; i++)
            {
                this.groupCache.Put(i, this.fixture.Create<SchemaGroupCacheItem>());
            }

            var result = await this.dataProvider.GetGroups(
                null,
                Array.Empty<Filter>(),
                SortOrder.Default("Id"),
                Pagination.Default());

            Assert.NotNull(result);
            Assert.Equal(groupsCount, result.Items.Count());
        }
    }
}
