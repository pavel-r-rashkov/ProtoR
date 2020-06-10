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
    public sealed class CategoryDataProviderTests : IDisposable
    {
        private readonly IgniteFixture igniteFixture;
        private readonly CategoryDataProvider dataProvider;
        private readonly ICache<long, CategoryCacheItem> categoryCache;
        private readonly Fixture fixture;

        public CategoryDataProviderTests(IgniteFixture igniteFixture)
        {
            this.igniteFixture = igniteFixture;
            this.dataProvider = new CategoryDataProvider(this.igniteFixture.IgniteFactory, this.igniteFixture.Configuration);
            this.categoryCache = this.igniteFixture.IgniteFactory
                .Instance()
                .GetCache<long, CategoryCacheItem>(this.igniteFixture.Configuration.Value.CategoryCacheName);

            this.fixture = new Fixture();
            this.fixture.Customizations.Add(new UtcRandomDateTimeSequenceGenerator());
        }

        public void Dispose()
        {
            this.igniteFixture.CleanIgniteCache();
        }

        [Fact]
        public async Task GetById_ShouldReturnCategory()
        {
            var cacheItem = this.fixture.Create<CategoryCacheItem>();
            var id = 1;
            await this.categoryCache.PutAsync(id, cacheItem);

            var category = await this.dataProvider.GetById(id);

            Assert.NotNull(category);
        }

        [Fact]
        public async Task GetCategories_ShouldReturnCategories()
        {
            var categoriesCount = 5;

            for (int i = 0; i < categoriesCount; i++)
            {
                var cacheItem = this.fixture.Create<CategoryCacheItem>();
                await this.categoryCache.PutAsync(i + 1, cacheItem);
            }

            var categories = await this.dataProvider.GetCategories();

            Assert.NotNull(categories);
            Assert.Equal(categoriesCount, categories.Count());
        }
    }
}
