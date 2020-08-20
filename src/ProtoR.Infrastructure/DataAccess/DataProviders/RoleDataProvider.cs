namespace ProtoR.Infrastructure.DataAccess.DataProviders
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Linq;
    using Microsoft.Extensions.Options;
    using ProtoR.Application.Common;
    using ProtoR.Application.Role;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public class RoleDataProvider : BaseDataProvider, IRoleDataProvider
    {
        private readonly ICache<long, RoleCacheItem> roleCache;
        private readonly ICache<RolePermissionKey, EmptyCacheItem> rolePermissionCache;

        public RoleDataProvider(
            IIgniteFactory igniteFactory,
            IOptions<IgniteExternalConfiguration> configurationProvider)
            : base(igniteFactory, configurationProvider)
        {
            this.roleCache = this.Ignite.GetOrCreateCache<long, RoleCacheItem>(this.ConfigurationProvider.CacheNames.RoleCacheName);
            this.rolePermissionCache = this.Ignite.GetOrCreateCache<RolePermissionKey, EmptyCacheItem>(this.ConfigurationProvider.CacheNames.RolePermissionCacheName);
        }

        public async Task<RoleDto> GetById(long id)
        {
            var result = await this.roleCache.TryGetAsync(id);

            if (!result.Success)
            {
                return null;
            }

            var roleCacheItem = result.Value;
            var permissionIds = this.rolePermissionCache
                .AsCacheQueryable()
                .Where(c => c.Key.RoleId == id)
                .Select(c => c.Key.PermissionId)
                .ToList();

            var role = this.MapToRoleDto(id, roleCacheItem);
            role.Permissions = permissionIds;

            return role;
        }

        public Task<PagedResult<RoleDto>> GetRoles(
            IEnumerable<Filter> filters,
            IEnumerable<SortOrder> sortOrders,
            Pagination pagination)
        {
            var permissionGroups = this.rolePermissionCache
                .AsCacheQueryable()
                .Select(c => new
                {
                    c.Key.RoleId,
                    c.Key.PermissionId,
                })
                .ToList()
                .GroupBy(c => c.RoleId)
                .ToDictionary(
                    c => c.Key,
                    c => c.Select(c => c.PermissionId).ToList());

            var roles = this.roleCache
                .AsCacheQueryable()
                .Select(r => new
                {
                    Id = r.Key,
                    r.Value.Name,
                    r.Value.CreatedBy,
                    r.Value.CreatedOn,
                })
                .Filter(filters);

            var totalCount = roles
                .Select(r => r.Id)
                .Count();

            var results = roles
                .Sort(sortOrders)
                .Page(pagination)
                .ToList()
                .Select(r =>
                {
                    var role = new RoleDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        CreatedBy = r.CreatedBy,
                        CreatedOn = r.CreatedOn,
                    };

                    if (permissionGroups.TryGetValue(r.Id, out var permissions))
                    {
                        role.Permissions = permissions;
                    }

                    return role;
                });

            return Task.FromResult(new PagedResult<RoleDto>(totalCount, results));
        }

        private RoleDto MapToRoleDto(long id, RoleCacheItem roleCacheItem)
        {
            return new RoleDto
            {
                Id = id,
                Name = roleCacheItem.Name,
                CreatedBy = roleCacheItem.CreatedBy,
                CreatedOn = roleCacheItem.CreatedOn,
            };
        }
    }
}
