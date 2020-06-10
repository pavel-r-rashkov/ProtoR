namespace ProtoR.Infrastructure.DataAccess.DataProviders
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Linq;
    using ProtoR.Application.Group;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public class GroupDataProvider : BaseDataProvider, IGroupDataProvider
    {
        private readonly ICache<long, SchemaGroupCacheItem> groupCache;

        public GroupDataProvider(
            IIgniteFactory igniteFactory,
            IIgniteConfiguration configurationProvider)
            : base(igniteFactory, configurationProvider)
        {
            this.groupCache = this.Ignite.GetCache<long, SchemaGroupCacheItem>(configurationProvider.SchemaGroupCacheName);
        }

        public Task<GroupDto> GetByName(string groupName)
        {
            var group = this.groupCache
                .AsCacheQueryable()
                .Where(c => c.Value.Name.ToUpper() == groupName.ToUpper())
                .Select(c => new
                {
                    Id = c.Key,
                    c.Value.Name,
                    c.Value.CategoryId,
                    c.Value.CreatedOn,
                    c.Value.CreatedBy,
                })
                .FirstOrDefault();

            if (group == null)
            {
                return Task.FromResult((GroupDto)null);
            }

            return Task.FromResult(new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                CategoryId = group.CategoryId,
                CreatedOn = group.CreatedOn,
                CreatedBy = group.CreatedBy,
            });
        }

        public Task<IEnumerable<GroupDto>> GetGroups(IEnumerable<long> categories)
        {
            var cacheQueryable = this.groupCache.AsCacheQueryable();

            if (categories != null)
            {
                cacheQueryable = cacheQueryable.Where(g => categories.Contains(g.Value.CategoryId));
            }

            var groups = cacheQueryable
                .Select(c => new
                {
                    Id = c.Key,
                    c.Value.Name,
                    c.Value.CategoryId,
                    c.Value.CreatedOn,
                    c.Value.CreatedBy,
                })
                .ToList();

            return Task.FromResult(groups.Select(g => new GroupDto
            {
                Id = g.Id,
                Name = g.Name,
                CategoryId = g.CategoryId,
                CreatedOn = g.CreatedOn,
                CreatedBy = g.CreatedBy,
            }));
        }

        public Task<long> GetCategoryId(long groupId)
        {
            var categoryId = this.groupCache
                .AsCacheQueryable()
                .Where(g => g.Key == groupId)
                .Select(g => g.Value.CategoryId)
                .FirstOrDefault();

            return Task.FromResult(categoryId);
        }
    }
}
