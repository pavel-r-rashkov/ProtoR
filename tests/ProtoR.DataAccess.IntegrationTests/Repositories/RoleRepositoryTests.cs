namespace ProtoR.DataAccess.IntegrationTests.Repositories
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Linq;
    using ProtoR.DataAccess.IntegrationTests.Fixtures;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Infrastructure.DataAccess.CacheItems;
    using ProtoR.Infrastructure.DataAccess.Repositories;
    using Xunit;

    [Collection(CollectionNames.IgniteCollection)]
    public sealed class RoleRepositoryTests : IDisposable
    {
        private readonly IgniteFixture igniteFixture;
        private readonly RoleRepository repository;
        private readonly ICache<long, RoleCacheItem> roleCache;
        private readonly ICache<RolePermissionKey, EmptyCacheItem> rolePermissionCache;
        private readonly ICache<UserRoleKey, EmptyCacheItem> userRoleCache;
        private readonly ICache<ClientRoleKey, EmptyCacheItem> clientRoleCache;

        public RoleRepositoryTests(IgniteFixture igniteFixture)
        {
            this.igniteFixture = igniteFixture;

            var userProvider = new UserProviderStub();
            this.repository = new RoleRepository(
                this.igniteFixture.IgniteFactory,
                this.igniteFixture.Configuration,
                userProvider);

            this.roleCache = this.igniteFixture.IgniteFactory
                .Instance()
                .GetCache<long, RoleCacheItem>(this.igniteFixture.Configuration.Value.RoleCacheName);

            this.rolePermissionCache = this.igniteFixture.IgniteFactory
                .Instance()
                .GetCache<RolePermissionKey, EmptyCacheItem>(this.igniteFixture.Configuration.Value.RolePermissionCacheName);

            this.userRoleCache = this.igniteFixture.IgniteFactory
                .Instance()
                .GetCache<UserRoleKey, EmptyCacheItem>(this.igniteFixture.Configuration.Value.UserRoleCacheName);

            this.clientRoleCache = this.igniteFixture.IgniteFactory
                .Instance()
                .GetCache<ClientRoleKey, EmptyCacheItem>(this.igniteFixture.Configuration.Value.ClientRoleCacheName);
        }

        public void Dispose()
        {
            this.igniteFixture.CleanIgniteCache();
        }

        [Fact]
        public async Task Add_ShouldInsertRole()
        {
            var name = "role name";
            var normalizedName = "ROLE NAME";
            var permissions = new Permission[] { Permission.GroupRead };
            var role = new Role(default, name, normalizedName, permissions);

            var roleId = await this.repository.Add(role);

            var roleCacheItem = await this.roleCache.GetAsync(roleId);
            var permissionCacheItems = this.rolePermissionCache
                .AsCacheQueryable()
                .Where(r => r.Key.RoleId == roleId)
                .Select(r => r.Key)
                .ToList();

            Assert.NotNull(roleCacheItem);
            Assert.Equal(name, roleCacheItem.Name);
            Assert.Equal(normalizedName, roleCacheItem.NormalizedName);
            Assert.NotEmpty(permissionCacheItems);
            Assert.Equal(roleId, permissionCacheItems.First().RoleId);
            Assert.Equal(permissions[0].Id, permissionCacheItems.First().PermissionId);
        }

        [Fact]
        public async Task Delete_ShouldRemoveRole()
        {
            var role = await this.InsertRole();

            await this.repository.Delete(role.Id);

            var roleCacheItem = await this.roleCache.TryGetAsync(role.Id);

            var rolePermissionCacheItems = this.rolePermissionCache
                .AsCacheQueryable()
                .Where(r => r.Key.RoleId == role.Id)
                .Select(r => r.Key)
                .ToList();

            var userRoleCacheItems = this.userRoleCache
                .AsCacheQueryable()
                .Where(r => r.Key.RoleId == role.Id)
                .Select(r => r.Key)
                .ToList();

            var clientRoleCacheItems = this.clientRoleCache
                .AsCacheQueryable()
                .Where(r => r.Key.RoleId == role.Id)
                .Select(r => r.Key)
                .ToList();

            Assert.False(roleCacheItem.Success);
            Assert.Empty(rolePermissionCacheItems);
            Assert.Empty(userRoleCacheItems);
            Assert.Empty(clientRoleCacheItems);
        }

        [Fact]
        public async Task GetById_ShouldReturnRole()
        {
            var insertedRole = await this.InsertRole();

            var role = await this.repository.GetById(insertedRole.Id);

            Assert.NotNull(role);
            Assert.NotEmpty(role.Permissions);
        }

        [Fact]
        public async Task GetRoles_ShouldReturnRoles()
        {
            await this.InsertRole(1);
            await this.InsertRole(2);

            var roles = await this.repository.GetRoles(new long[] { 1, 2 });

            Assert.Equal(2, roles.Count());
        }

        [Fact]
        public async Task GetByName_ShouldReturnRole()
        {
            var insertedRole = await this.InsertRole();

            var role = await this.repository.GetByName(insertedRole.NormalizedName);

            Assert.NotNull(role);
        }

        [Fact]
        public async Task Update_ShouldUpdateRole()
        {
            var role = await this.InsertRole();
            var newName = "new name";
            var newNormalizedName = "NEW NAME";
            var newPermissions = new Permission[] { Permission.SchemaRead };

            role.Name = newName;
            role.NormalizedName = newNormalizedName;
            role.AssignPermissions(newPermissions);
            await this.repository.Update(role);

            var roleCacheItem = await this.roleCache.GetAsync(role.Id);
            var rolePermissionCacheItems = this.rolePermissionCache
                .AsCacheQueryable()
                .Where(r => r.Key.RoleId == role.Id)
                .Select(r => r.Key)
                .ToList();

            Assert.Equal(newName, roleCacheItem.Name);
            Assert.Equal(newNormalizedName, roleCacheItem.NormalizedName);
            Assert.Single(rolePermissionCacheItems);
            Assert.Equal(Permission.SchemaRead.Id, rolePermissionCacheItems.First().PermissionId);
        }

        private async Task<Role> InsertRole(long roleId = 1)
        {
            var name = "role name";
            var normalizedName = "ROLE NAME";
            var permissions = new Permission[] { Permission.GroupRead };
            var role = new Role(roleId, name, normalizedName, permissions);

            await this.roleCache.PutAsync(roleId, new RoleCacheItem
            {
                Name = name,
                NormalizedName = normalizedName,
                CreatedBy = "test user",
                CreatedOn = DateTime.UtcNow,
            });

            await this.userRoleCache.PutAsync(
                new UserRoleKey { RoleId = roleId, UserId = 1 },
                new EmptyCacheItem());

            await this.clientRoleCache.PutAsync(
                new ClientRoleKey { RoleId = roleId, ClientId = 1 },
                new EmptyCacheItem());

            await this.rolePermissionCache.PutAsync(
                new RolePermissionKey { RoleId = roleId, PermissionId = Permission.GroupRead.Id },
                new EmptyCacheItem());

            return role;
        }
    }
}
