namespace ProtoR.Infrastructure.DataAccess.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Core.DataStructures;
    using Apache.Ignite.Linq;
    using ProtoR.Application;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public class RoleRepository : BaseRepository, IRoleRepository
    {
        private readonly ICache<long, RoleCacheItem> roleCache;
        private readonly ICache<RolePermissionKey, EmptyCacheItem> rolePermissionCache;
        private readonly ICache<UserRoleKey, EmptyCacheItem> userRoleCache;
        private readonly ICache<ClientRoleKey, EmptyCacheItem> clientRoleCache;

        public RoleRepository(
            IIgniteFactory igniteFactory,
            IIgniteConfiguration configurationProvider,
            IUserProvider userProvider)
            : base(igniteFactory, configurationProvider, userProvider)
        {
            this.roleCache = this.Ignite.GetOrCreateCache<long, RoleCacheItem>(this.ConfigurationProvider.RoleCacheName);
            this.rolePermissionCache = this.Ignite.GetOrCreateCache<RolePermissionKey, EmptyCacheItem>(this.ConfigurationProvider.RolePermissionCacheName);
            this.userRoleCache = this.Ignite.GetOrCreateCache<UserRoleKey, EmptyCacheItem>(this.ConfigurationProvider.UserRoleCacheName);
            this.clientRoleCache = this.Ignite.GetOrCreateCache<ClientRoleKey, EmptyCacheItem>(this.ConfigurationProvider.ClientRoleCacheName);
        }

        public async Task<long> Add(Role role)
        {
            _ = role ?? throw new ArgumentNullException(nameof(role));

            IAtomicSequence roleIdGenerator = this.Ignite.GetAtomicSequence(
                $"{typeof(RoleCacheItem).Name.ToUpperInvariant()}{CacheConstants.IdSequenceSufix}",
                0,
                true);

            var id = roleIdGenerator.Increment();
            await this.roleCache.PutIfAbsentAsync(
                id,
                new RoleCacheItem
                {
                    Name = role.Name,
                    NormalizedName = role.NormalizedName,
                    CreatedBy = this.UserProvider.GetCurrentUserName(),
                    CreatedOn = DateTime.UtcNow,
                });

            var newPermissions = role.Permissions
                .Select(p => new KeyValuePair<RolePermissionKey, EmptyCacheItem>(
                    new RolePermissionKey
                    {
                        RoleId = id,
                        PermissionId = p.Id,
                    },
                    new EmptyCacheItem()));

            await this.rolePermissionCache.PutAllAsync(newPermissions);

            return id;
        }

        public async Task Delete(long id)
        {
            await this.roleCache.RemoveAsync(id);

            var rolePermissionsToDelete = this.rolePermissionCache
                .AsCacheQueryable()
                .Where(r => r.Key.RoleId == id)
                .Select(r => r.Key)
                .ToList();

            var userRolesToDelete = this.userRoleCache.AsCacheQueryable()
                .Where(ur => ur.Key.RoleId == id)
                .Select(ur => ur.Key);

            var clientRolesToDelete = this.clientRoleCache.AsCacheQueryable()
                .Where(ur => ur.Key.RoleId == id)
                .Select(ur => ur.Key);

            await this.rolePermissionCache.RemoveAllAsync(rolePermissionsToDelete);
            await this.userRoleCache.RemoveAllAsync(userRolesToDelete);
            await this.clientRoleCache.RemoveAllAsync(clientRolesToDelete);
        }

        public async Task<Role> GetById(long id)
        {
            var result = await this.roleCache.TryGetAsync(id);

            if (!result.Success)
            {
                return null;
            }

            var role = result.Value;

            return new Role(id, role.Name, role.NormalizedName, this.GetRolePermissions(id));
        }

        public async Task<IEnumerable<Role>> GetRoles(IEnumerable<long> ids)
        {
            var rolePermissions = this.rolePermissionCache
                .AsCacheQueryable()
                .Where(r => ids.Contains(r.Key.RoleId))
                .ToList()
                .GroupBy(r => r.Key.RoleId)
                .ToDictionary(g => g.Key, g => g.Select(g => Permission.FromId(g.Key.PermissionId)));

            var roles = (await this.roleCache
                .GetAllAsync(ids))
                .Select(r => new Role(
                    r.Key,
                    r.Value.Name,
                    r.Value.NormalizedName,
                    rolePermissions.ContainsKey(r.Key) ? rolePermissions[r.Key] : Array.Empty<Permission>()));

            return roles;
        }

        public Task<Role> GetByName(string normalizedName)
        {
            var role = this.roleCache
                .AsCacheQueryable()
                .Where(u => u.Value.NormalizedName == normalizedName)
                .FirstOrDefault();

            if (role == null)
            {
                return Task.FromResult((Role)null);
            }

            return Task.FromResult(new Role(
                role.Key,
                role.Value.Name,
                role.Value.NormalizedName,
                this.GetRolePermissions(role.Key)));
        }

        public async Task Update(Role role)
        {
            _ = role ?? throw new ArgumentNullException(nameof(role));

            var roleCacheItem = await this.roleCache.GetAsync(role.Id);
            roleCacheItem.Name = role.Name;
            roleCacheItem.NormalizedName = role.NormalizedName;

            await this.roleCache.PutAsync(role.Id, roleCacheItem);

            var existingPermissions = this.rolePermissionCache
                .AsCacheQueryable()
                .Where(r => r.Key.RoleId == role.Id)
                .ToList();

            var permissionsToRemove = existingPermissions
                .Where(p => !role.Permissions
                    .Select(permission => permission.Id)
                    .Contains(p.Key.PermissionId))
                .Select(p => p.Key);

            this.rolePermissionCache.RemoveAll(permissionsToRemove);

            var newPermissions = role.Permissions
                .Where(p => !existingPermissions
                    .Select(permission => permission.Key.PermissionId)
                    .Contains(p.Id))
                .Select(p => new KeyValuePair<RolePermissionKey, EmptyCacheItem>(
                    new RolePermissionKey
                    {
                        RoleId = role.Id,
                        PermissionId = p.Id,
                    },
                    new EmptyCacheItem()));

            await this.rolePermissionCache.PutAllAsync(newPermissions);
        }

        private IEnumerable<Permission> GetRolePermissions(long roleId)
        {
            return this.rolePermissionCache
                .AsCacheQueryable()
                .Where(c => c.Key.RoleId == roleId)
                .Select(c => c.Key.PermissionId)
                .ToList()
                .Select(permissionId => Permission.FromId(permissionId));
        }
    }
}
