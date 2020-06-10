namespace ProtoR.Infrastructure.DataAccess.Repositories
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Linq;
    using ProtoR.Application;
    using ProtoR.Domain.CategoryAggregate;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        private readonly ICache<long, CategoryCacheItem> categoryCache;

        public CategoryRepository(
            IIgniteFactory igniteFactory,
            IIgniteConfiguration configurationProvider,
            IUserProvider userProvider)
            : base(igniteFactory, configurationProvider, userProvider)
        {
            this.categoryCache = this.Ignite.GetOrCreateCache<long, CategoryCacheItem>(configurationProvider.CategoryCacheName);
        }

        public async Task<long> Add(Category category)
        {
            _ = category ?? throw new ArgumentNullException(nameof(category));

            var categoryIdGenerator = this.Ignite.GetAtomicSequence(
                $"{typeof(CategoryCacheItem).Name.ToUpperInvariant()}{CacheConstants.IdSequenceSufix}",
                1,
                true);

            var id = category.Id == default ? categoryIdGenerator.Increment() : category.Id;
            await this.categoryCache.PutIfAbsentAsync(
                id,
                new CategoryCacheItem
                {
                    Name = category.Name,
                    CreatedBy = this.UserProvider.GetCurrentUserName(),
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
            _ = category ?? throw new ArgumentNullException(nameof(category));

            var categoryCacheItem = await this.categoryCache.GetAsync(category.Id);
            categoryCacheItem.Name = category.Name;

            await this.categoryCache.PutAsync(category.Id, categoryCacheItem);
        }
    }
}
