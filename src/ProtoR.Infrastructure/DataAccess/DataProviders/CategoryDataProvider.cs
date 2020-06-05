namespace ProtoR.Infrastructure.DataAccess.DataProviders
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Linq;
    using ProtoR.Application.Category;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public class CategoryDataProvider : ICategoryDataProvider
    {
        private readonly IIgnite ignite;
        private readonly ICache<long, CategoryCacheItem> categoryCache;

        public CategoryDataProvider(
            IIgniteFactory igniteFactory,
            IIgniteConfiguration configurationProvider)
        {
            this.ignite = igniteFactory.Instance();
            this.categoryCache = this.ignite.GetOrCreateCache<long, CategoryCacheItem>(configurationProvider.CategoryCacheName);
        }

        public async Task<CategoryDto> GetById(long id)
        {
            var result = await this.categoryCache.TryGetAsync(id);

            if (!result.Success)
            {
                return null;
            }

            var category = result.Value;

            return new CategoryDto
            {
                Id = id,
                Name = category.Name,
                CreatedBy = category.CreatedBy,
                CreatedOn = category.CreatedOn,
            };
        }

        public Task<IEnumerable<CategoryDto>> GetCategories()
        {
            var categories = this.categoryCache.AsCacheQueryable().ToList();

            return Task.FromResult(categories.Select(c => new CategoryDto
            {
                Id = c.Key,
                Name = c.Value.Name,
                CreatedOn = c.Value.CreatedOn,
                CreatedBy = c.Value.CreatedBy,
            }));
        }
    }
}