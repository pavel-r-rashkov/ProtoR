namespace ProtoR.DataAccess.IntegrationTests.Repositories
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Linq;
    using ProtoR.DataAccess.IntegrationTests.Fixtures;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.UserAggregate;
    using ProtoR.Infrastructure.DataAccess.CacheItems;
    using ProtoR.Infrastructure.DataAccess.Repositories;
    using Xunit;

    [Collection(CollectionNames.IgniteCollection)]
    public sealed class UserRepositoryTests : IDisposable
    {
        private readonly IgniteFixture igniteFixture;
        private readonly UserRepository repository;
        private readonly ICache<long, UserCacheItem> userCache;
        private readonly ICache<UserRoleKey, EmptyCacheItem> userRoleCache;

        public UserRepositoryTests(IgniteFixture igniteFixture)
        {
            this.igniteFixture = igniteFixture;

            var userProviderStub = new UserProviderStub();
            this.repository = new UserRepository(
                this.igniteFixture.IgniteFactory,
                this.igniteFixture.Configuration,
                userProviderStub);

            this.userCache = this.igniteFixture.IgniteFactory
                .Instance()
                .GetCache<long, UserCacheItem>(this.igniteFixture.Configuration.Value.UserCacheName);

            this.userRoleCache = this.igniteFixture.IgniteFactory
                .Instance()
                .GetCache<UserRoleKey, EmptyCacheItem>(this.igniteFixture.Configuration.Value.UserRoleCacheName);
        }

        public void Dispose()
        {
            this.igniteFixture.CleanIgniteCache();
        }

        [Fact]
        public async Task Add_ShouldInsertUser()
        {
            var userName = "test user";
            var normalizedName = "TEST USER";
            var passwordHash = "abc123";
            var roleId = 1;
            long id = default;
            var groupRestriction = "Zxc*";
            var isActive = true;

            var user = new User(
                id,
                userName,
                normalizedName,
                passwordHash,
                isActive,
                new GroupRestriction[] { new GroupRestriction(groupRestriction) },
                new RoleBinding[] { new RoleBinding(roleId, id, null) });

            var userId = await this.repository.Add(user);

            var userCacheItem = await this.userCache.GetAsync(userId);
            Assert.NotNull(userCacheItem);
            Assert.Equal(userName, userCacheItem.UserName);
            Assert.Equal(normalizedName, userCacheItem.NormalizedUserName);
            Assert.Equal(passwordHash, userCacheItem.PasswordHash);
            Assert.Equal(groupRestriction, userCacheItem.GroupRestrictions);

            var roleBinding = this.userRoleCache
                .AsCacheQueryable()
                .Where(r => r.Key.UserId == userId)
                .Select(r => r.Key)
                .FirstOrDefault();

            Assert.NotNull(roleBinding);
            Assert.Equal(roleId, roleBinding.RoleId);
            Assert.Equal(userId, roleBinding.UserId);
        }

        [Fact]
        public async Task Update_ShouldUpdateUser()
        {
            var user = await this.InsertUser();
            var newPasswordHash = "testhash";
            var removedRoleId = 1;
            var newRoleId = 2;
            var newGroupRestriction = "Qwer*";
            var newActiveState = false;

            user.PasswordHash = newPasswordHash;
            user.GroupRestrictions = new GroupRestriction[] { new GroupRestriction(newGroupRestriction) };
            user.IsActive = newActiveState;
            user.AddRole(newRoleId);
            user.RemoveRole(removedRoleId);

            await this.repository.Update(user);

            var userCacheItem = await this.userCache.GetAsync(user.Id);

            var roleBindings = this.userRoleCache
                .AsCacheQueryable()
                .Where(r => r.Key.UserId == user.Id)
                .Select(r => r.Key)
                .ToList();

            Assert.Equal(newPasswordHash, userCacheItem.PasswordHash);
            Assert.Equal(newGroupRestriction, userCacheItem.GroupRestrictions);
            Assert.Equal(newActiveState, userCacheItem.IsActive);

            Assert.Single(roleBindings);
            Assert.Equal(newRoleId, roleBindings.First().RoleId);
        }

        [Fact]
        public async Task Delete_ShouldDeleteUser()
        {
            var user = await this.InsertUser();

            await this.repository.Delete(user.Id);

            var userCacheItem = await this.userCache.TryGetAsync(user.Id);

            var roleBindings = this.userRoleCache
                .AsCacheQueryable()
                .Where(r => r.Key.UserId == user.Id)
                .Select(r => r.Key)
                .ToList();

            Assert.False(userCacheItem.Success);
            Assert.Empty(roleBindings);
        }

        [Fact]
        public async Task GetById_ShouldReturnUser()
        {
            var insertedUser = await this.InsertUser();

            var user = await this.repository.GetById(insertedUser.Id);

            Assert.NotNull(user);
            Assert.NotNull(user.UserName);
            Assert.NotNull(user.NormalizedUserName);
            Assert.NotNull(user.PasswordHash);
            Assert.True(user.IsActive);
            Assert.NotEmpty(user.RoleBindings);
            Assert.NotEmpty(user.GroupRestrictions);
        }

        [Fact]
        public async Task GetByName_ShouldReturnUser()
        {
            var insertedUser = await this.InsertUser();

            var user = await this.repository.GetByName(insertedUser.NormalizedUserName);

            Assert.NotNull(user);
            Assert.NotNull(user.UserName);
            Assert.NotNull(user.NormalizedUserName);
            Assert.NotNull(user.PasswordHash);
            Assert.True(user.IsActive);
            Assert.NotEmpty(user.RoleBindings);
            Assert.NotEmpty(user.GroupRestrictions);
        }

        [Fact]
        public async Task GetUsersInRole_ShouldReturnUsers()
        {
            await this.InsertUser();
            await this.InsertUser(2);
            var roleId = 1;

            var users = await this.repository.GetUsersInRole(roleId);

            Assert.Equal(2, users.Count());
        }

        private async Task<User> InsertUser(long userId = 1)
        {
            var roleId = 1;
            var userName = "test user";
            var normalizedUserName = "TEST USER";
            var passwordHash = "abc123";
            var groupRestriction = "*";
            var isActive = true;

            await this.userCache.PutAsync(userId, new UserCacheItem
            {
                UserName = userName,
                NormalizedUserName = normalizedUserName,
                PasswordHash = passwordHash,
                IsActive = isActive,
                GroupRestrictions = groupRestriction,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "test user",
            });

            await this.userRoleCache.PutAsync(new UserRoleKey { UserId = userId, RoleId = roleId }, new EmptyCacheItem());

            var user = new User(
                userId,
                userName,
                normalizedUserName,
                passwordHash,
                isActive,
                new GroupRestriction[] { new GroupRestriction(groupRestriction) },
                new RoleBinding[] { new RoleBinding(roleId, userId, null) });

            return user;
        }
    }
}
