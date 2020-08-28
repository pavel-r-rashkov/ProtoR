namespace ProtoR.DataAccess.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Linq;
    using AutoFixture;
    using IdentityServer4.Models;
    using ProtoR.DataAccess.IntegrationTests.Fixtures;
    using ProtoR.Infrastructure.DataAccess;
    using ProtoR.Infrastructure.DataAccess.CacheItems;
    using Xunit;

    [Collection(CollectionNames.IgniteCollection)]
    public sealed class IgniteGrantStoreTests : IDisposable
    {
        private readonly IgniteFixture igniteFixture;
        private readonly ICache<string, GrantCacheItem> grantCache;
        private readonly Fixture fixture;
        private readonly IgniteGrantStore grantStore;

        public IgniteGrantStoreTests(IgniteFixture igniteFixture)
        {
            this.igniteFixture = igniteFixture;
            var cacheNames = this.igniteFixture.Configuration.Value.CacheNames;

            this.grantStore = new IgniteGrantStore(
                igniteFixture.IgniteFactory,
                igniteFixture.Configuration,
                new UserProviderStub());

            this.grantCache = this.igniteFixture.IgniteFactory
                .Instance()
                .GetCache<string, GrantCacheItem>(cacheNames.GrantStoreCacheName);

            this.fixture = new Fixture();
            this.fixture.Customizations.Add(new UtcRandomDateTimeSequenceGenerator());
        }

        public void Dispose()
        {
            this.igniteFixture.CleanIgniteCache();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnGrants()
        {
            var subjectId = "test-subject";
            var subjectGrantsCount = 3;

            for (int i = 0; i < 5; i++)
            {
                var cacheItem = this.fixture.Create<GrantCacheItem>();

                if (i < subjectGrantsCount)
                {
                    cacheItem.SubjectId = subjectId;
                }

                await this.grantCache.PutAsync(i.ToString(CultureInfo.InvariantCulture), cacheItem);
            }

            var subjectGrants = await this.grantStore.GetAllAsync(subjectId);

            Assert.Equal(subjectGrantsCount, subjectGrants.Count());
        }

        [Fact]
        public async Task GetAsync_ShouldReturnGrant()
        {
            var grantKey = "grant-key";
            var cacheItem = this.fixture.Create<GrantCacheItem>();
            await this.grantCache.PutAsync(grantKey, cacheItem);

            var grant = await this.grantStore.GetAsync(grantKey);

            Assert.NotNull(grant);
        }

        [Fact]
        public async Task RemoveAllAsync_BySubjectIdAndClientId_ShouldRemoveMatchingGrants()
        {
            var id = 0;
            var totalNumberOfGrants = 10;
            var cacheItems = this.fixture
                .CreateMany<GrantCacheItem>(totalNumberOfGrants)
                .Select(g => new KeyValuePair<string, GrantCacheItem>((++id).ToString(CultureInfo.InvariantCulture), g))
                .ToList();

            var subjectId = "subject-to-remove";
            var clientId = "client-to-remove";

            cacheItems[0].Value.SubjectId = subjectId;
            cacheItems[0].Value.ClientId = clientId;
            cacheItems[1].Value.SubjectId = subjectId;
            cacheItems[2].Value.ClientId = clientId;

            await this.grantCache.PutAllAsync(cacheItems);

            await this.grantStore.RemoveAllAsync(subjectId, clientId);

            var currentGrantCount = this.grantCache
                .AsCacheQueryable()
                .ToList()
                .Count;

            Assert.Equal(totalNumberOfGrants - 1, currentGrantCount);
        }

        [Fact]
        public async Task RemoveAllAsync_BySubjectIdClientIdAndType_ShouldRemoveMatchingGrants()
        {
            var id = 0;
            var totalNumberOfGrants = 10;
            var cacheItems = this.fixture
                .CreateMany<GrantCacheItem>(totalNumberOfGrants)
                .Select(g => new KeyValuePair<string, GrantCacheItem>((++id).ToString(CultureInfo.InvariantCulture), g))
                .ToList();

            var subjectId = "subject-to-remove";
            var clientId = "client-to-remove";
            var type = "type-to-remove";

            cacheItems[0].Value.SubjectId = subjectId;
            cacheItems[0].Value.ClientId = clientId;
            cacheItems[0].Value.Type = type;
            cacheItems[1].Value.SubjectId = subjectId;
            cacheItems[1].Value.ClientId = clientId;
            cacheItems[2].Value.Type = type;
            cacheItems[2].Value.ClientId = clientId;

            await this.grantCache.PutAllAsync(cacheItems);

            await this.grantStore.RemoveAllAsync(subjectId, clientId, type);

            var currentGrantCount = this.grantCache
                .AsCacheQueryable()
                .ToList()
                .Count;

            Assert.Equal(totalNumberOfGrants - 1, currentGrantCount);
        }

        [Fact]
        public async Task RemoveAsync_ByKey_ShouldRemoveMatchingGrant()
        {
            var id = 0;
            var keyToRemove = "key-to-remove";
            var totalNumberOfGrants = 10;
            var cacheItems = this.fixture
                .CreateMany<GrantCacheItem>(totalNumberOfGrants)
                .Select(g =>
                {
                    var key = id++ == 0 ? keyToRemove : id.ToString(CultureInfo.InvariantCulture);
                    return new KeyValuePair<string, GrantCacheItem>(key, g);
                })
                .ToList();

            await this.grantCache.PutAllAsync(cacheItems);

            await this.grantStore.RemoveAsync(keyToRemove);

            var currentGrantCount = this.grantCache
                .AsCacheQueryable()
                .ToList()
                .Count;

            Assert.Equal(totalNumberOfGrants - 1, currentGrantCount);
        }

        [Fact]
        public async Task StoreAsync_ShouldPersistGrant()
        {
            var grant = this.fixture.Create<PersistedGrant>();

            await this.grantStore.StoreAsync(grant);

            var grantCacheItem = await this.grantCache.GetAsync(grant.Key);

            Assert.NotNull(grantCacheItem);
        }
    }
}
