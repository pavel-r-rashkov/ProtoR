namespace ProtoR.DataAccess.IntegrationTests.Repositories
{
    using System;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using ProtoR.DataAccess.IntegrationTests.Fixtures;
    using ProtoR.Domain.CategoryAggregate;
    using ProtoR.Infrastructure.DataAccess.CacheItems;
    using ProtoR.Infrastructure.DataAccess.Repositories;
    using Xunit;

    [Collection(CollectionNames.IgniteCollection)]
    public sealed class CategoryRepositroyTests : IDisposable
    {
        private readonly IgniteFixture igniteFixture;
        private readonly CategoryRepository repository;
        private readonly ICache<long, CategoryCacheItem> categoryCache;

        public CategoryRepositroyTests(IgniteFixture igniteFixture)
        {
            this.igniteFixture = igniteFixture;

            var userProvider = new UserProviderStub();
            this.repository = new CategoryRepository(
                this.igniteFixture.IgniteFactory,
                this.igniteFixture.Configuration,
                userProvider);

            this.categoryCache = this.igniteFixture.IgniteFactory.Instance().GetCache<long, CategoryCacheItem>(this.igniteFixture.Configuration.CategoryCacheName);
        }

        public void Dispose()
        {
            this.igniteFixture.CleanIgniteCache();
        }

        [Fact]
        public async Task Add_ShouldInsertCategory()
        {
            var name = "test category";
            var category = new Category(name);

            var categoryId = await this.repository.Add(category);

            var categoryCacheItem = await this.categoryCache.GetAsync(categoryId);
            Assert.NotNull(categoryCacheItem);
            Assert.Equal(name, categoryCacheItem.Name);
            Assert.NotNull(categoryCacheItem.CreatedBy);
        }

        [Fact]
        public async Task Delete_ShouldRemoveCategory()
        {
            var insertedCategory = await this.InsertCategory();

            await this.repository.Delete(insertedCategory.Id);

            var categoryCacheItem = await this.categoryCache.TryGetAsync(insertedCategory.Id);
            Assert.False(categoryCacheItem.Success);
        }

        [Fact]
        public async Task GetById_ShouldReturnCategory()
        {
            var insertedCategory = await this.InsertCategory();

            var category = await this.repository.GetById(insertedCategory.Id);

            Assert.NotNull(category);
            Assert.Equal(insertedCategory.Name, category.Name);
        }

        [Fact]
        public async Task GetByName_ShouldReturnCategory()
        {
            var insertedCategory = await this.InsertCategory();

            var category = await this.repository.GetByName(insertedCategory.Name);

            Assert.NotNull(category);
        }

        [Fact]
        public async Task Update_ShouldUpdateCategory()
        {
            var insertedCategory = await this.InsertCategory();
            var newName = "updated name";
            insertedCategory.Name = newName;

            await this.repository.Update(insertedCategory);

            var categoryCacheItem = await this.categoryCache.GetAsync(insertedCategory.Id);
            Assert.Equal(newName, categoryCacheItem.Name);
        }

        private async Task<Category> InsertCategory()
        {
            var categoryId = 1;
            var name = "category name";
            var category = new Category(categoryId, name);

            await this.categoryCache.PutAsync(categoryId, new CategoryCacheItem
            {
                Name = name,
                CreatedBy = "test user",
                CreatedOn = DateTime.UtcNow,
            });

            return category;
        }
    }
}
