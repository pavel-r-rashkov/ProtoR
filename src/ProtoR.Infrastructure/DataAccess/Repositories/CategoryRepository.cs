namespace ProtoR.Infrastructure.DataAccess.Repositories
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Core.DataStructures;
    using Apache.Ignite.Linq;
    using ProtoR.Application;
    using ProtoR.Domain.CategoryAggregate;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public class CategoryRepository : ICategoryRepository
    {
        private readonly IIgnite ignite;
        private readonly ICache<long, CategoryCacheItem> categoryCache;
        private readonly IUserProvider userProvider;

        public CategoryRepository(
            IIgniteFactory igniteFactory,
            IIgniteConfiguration configurationProvider,
            IUserProvider userProvider)
        {
            this.ignite = igniteFactory.Instance();
            this.categoryCache = this.ignite.GetOrCreateCache<long, CategoryCacheItem>(configurationProvider.CategoryCacheName);
            this.userProvider = userProvider;
        }

        public async Task<long> Add(Category category)
        {
            var categoryIdGenerator = this.ignite.GetAtomicSequence(
                $"{typeof(CategoryCacheItem).Name.ToUpperInvariant()}{CacheConstants.IdSequenceSufix}",
                1,
                true);

            var id = category.Id == default ? categoryIdGenerator.Increment() : category.Id;
            await this.categoryCache.PutIfAbsentAsync(
                id,
                new CategoryCacheItem
                {
                    Name = category.Name,
                    CreatedBy = this.userProvider.GetCurrentUserName(),
                    CreatedOn = DateTime.UtcNow,
                });

            return id;
        }

        public async Task Delete(long id)
        {
            await this.categoryCache.RemoveAsync(id);
        }

        public async Task<Category> GetById(long id)
        {
            var result = await this.categoryCache.TryGetAsync(id);

            if (!result.Success)
            {
                return null;
            }

            return new Category(id, result.Value.Name);
        }

        public Task<Category> GetByName(string name)
        {
            var category = this.categoryCache
                .AsCacheQueryable()
                .Where(u => u.Value.Name.ToUpper() == name.ToUpper())
                .FirstOrDefault();

            if (category == null)
            {
                return Task.FromResult((Category)null);
            }

            return Task.FromResult(new Category(
                category.Key,
                category.Value.Name));
        }

        public async Task Update(Category category)
        {
            var categoryCacheItem = await this.categoryCache.GetAsync(category.Id);
            categoryCacheItem.Name = category.Name;

            await this.categoryCache.PutAsync(category.Id, categoryCacheItem);
        }
    }
}
