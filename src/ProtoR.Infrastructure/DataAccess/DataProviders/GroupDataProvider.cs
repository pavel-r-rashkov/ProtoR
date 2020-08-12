namespace ProtoR.Infrastructure.DataAccess.DataProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Linq;
    using Microsoft.Extensions.Options;
    using ProtoR.Application.Common;
    using ProtoR.Application.Group;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public class GroupDataProvider : BaseDataProvider, IGroupDataProvider
    {
        private readonly ICache<long, SchemaGroupCacheItem> groupCache;

        public GroupDataProvider(
            IIgniteFactory igniteFactory,
            IOptions<IgniteExternalConfiguration> configurationProvider)
            : base(igniteFactory, configurationProvider)
        {
            this.groupCache = this.Ignite.GetCache<long, SchemaGroupCacheItem>(this.ConfigurationProvider.SchemaGroupCacheName);
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
                CreatedOn = group.CreatedOn,
                CreatedBy = group.CreatedBy,
            });
        }

        public Task<PagedResult<GroupDto>> GetGroups(
            Expression<Func<GroupDto, bool>> filter,
            IEnumerable<Filter> filters,
            IEnumerable<SortOrder> sortOrders,
            Pagination pagination)
        {
            var groups = this.groupCache
                .AsCacheQueryable()
                .Select(c => new
                {
                    Id = c.Key,
                    c.Value.Name,
                    c.Value.CreatedOn,
                    c.Value.CreatedBy,
                });

            if (filter != null)
            {
                groups = groups.WhereWithTypeConversion(filter);
            }

            groups = groups.Filter(filters);
            var totalCount = groups
                .Select(g => g.Id)
                .Count();

            groups = groups
                .Sort(sortOrders)
                .Page(pagination);

            var results = groups
                .ToList()
                .Select(g => new GroupDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    CreatedOn = g.CreatedOn,
                    CreatedBy = g.CreatedBy,
                });

            return Task.FromResult(new PagedResult<GroupDto>(totalCount, results));
        }

        public Task<string> GetGroupNameById(long id)
        {
            var groupName = this.groupCache
                .AsCacheQueryable()
                .Where(c => c.Key == id)
                .Select(c => c.Value.Name)
                .FirstOrDefault();

            return Task.FromResult(groupName);
        }
    }
}
