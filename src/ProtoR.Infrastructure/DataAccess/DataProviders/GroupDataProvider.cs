namespace ProtoR.Infrastructure.DataAccess.DataProviders
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Linq;
    using ProtoR.Application.Group;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public class GroupDataProvider : IGroupDataProvider
    {
        private readonly IIgnite ignite;
        private readonly string groupCacheName;

        public GroupDataProvider(
            IIgnite ignite,
            IIgniteConfigurationProvider configurationProvider)
        {
            this.ignite = ignite;
            this.groupCacheName = configurationProvider.SchemaGroupCacheName;
        }

        public Task<GroupDto> GetByName(string groupName)
        {
            ICache<long, SchemaGroupCacheItem> groupCache = this.ignite.GetCache<long, SchemaGroupCacheItem>(this.groupCacheName);
            var group = groupCache
                .AsCacheQueryable()
                .Where(c => c.Value.Name.ToUpper() == groupName.ToUpper())
                .Select(c => new
                {
                    Id = c.Key,
                    c.Value.Name,
                    c.Value.CreatedOn,
                    c.Value.CreatedBy,
                })
                .FirstOrDefault();

            return Task.FromResult(new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                CreatedOn = group.CreatedOn,
                CreatedBy = group.CreatedBy,
            });
        }

        public Task<IEnumerable<GroupDto>> GetGroups()
        {
            ICache<long, SchemaGroupCacheItem> groupCache = this.ignite.GetCache<long, SchemaGroupCacheItem>(this.groupCacheName);
            var groups = groupCache
                .AsCacheQueryable()
                .Select(c => new
                {
                    Id = c.Key,
                    c.Value.Name,
                    c.Value.CreatedOn,
                    c.Value.CreatedBy,
                })
                .ToList();

            return Task.FromResult(groups.Select(g => new GroupDto
            {
                Id = g.Id,
                Name = g.Name,
                CreatedOn = g.CreatedOn,
                CreatedBy = g.CreatedBy,
            }));
        }
    }
}
