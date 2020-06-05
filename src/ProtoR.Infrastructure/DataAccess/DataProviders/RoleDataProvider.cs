namespace ProtoR.Infrastructure.DataAccess.DataProviders
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Linq;
    using ProtoR.Application.Role;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public class RoleDataProvider : IRoleDataProvider
    {
        private readonly IIgnite ignite;
        private readonly ICache<long, RoleCacheItem> roleCache;
        private readonly ICache<RolePermissionKey, EmptyCacheItem> rolePermissionCache;

        public RoleDataProvider(
            IIgniteFactory igniteFactory,
            IIgniteConfiguration configurationProvider)
        {
            this.ignite = igniteFactory.Instance();
            this.roleCache = this.ignite.GetOrCreateCache<long, RoleCacheItem>(configurationProvider.RoleCacheName);
            this.rolePermissionCache = this.ignite.GetOrCreateCache<RolePermissionKey, EmptyCacheItem>(configurationProvider.RolePermissionCacheName);
        }

        public async Task<RoleDto> GetById(long id)
        {
            var roleCacheItem = await this.roleCache.GetAsync(id);
            var permissionIds = this.rolePermissionCache
                .AsCacheQueryable()
                .Where(c => c.Key.RoleId == id)
                .Select(c => c.Key.PermissionId)
                .ToList();

            var role = this.MapToRoleDto(id, roleCacheItem);
            role.Permissions = permissionIds;

            return role;
        }

        public Task<IEnumerable<RoleDto>> GetRoles()
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
                .ToList()
                .Select(c =>
                {
                    var role = this.MapToRoleDto(c.Key, c.Value);

                    if (permissionGroups.TryGetValue(c.Key, out var permissions))
                    {
                        role.Permissions = permissions;
                    }

                    return role;
                });

            return Task.FromResult(roles);
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
