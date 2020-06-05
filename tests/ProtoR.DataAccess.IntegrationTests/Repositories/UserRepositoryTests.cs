namespace ProtoR.DataAccess.IntegrationTests.Repositories
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Linq;
    using ProtoR.DataAccess.IntegrationTests.Fixtures;
    using ProtoR.Domain.CategoryAggregate;
    using ProtoR.Domain.RoleAggregate;
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
        private readonly ICache<UserCategoryKey, EmptyCacheItem> userCategoryCache;

        public UserRepositoryTests(IgniteFixture igniteFixture)
        {
            this.igniteFixture = igniteFixture;

            var userProviderStub = new UserProviderStub();
            this.repository = new UserRepository(
                this.igniteFixture.IgniteFactory,
                this.igniteFixture.Configuration,
                userProviderStub);

            this.userCache = this.igniteFixture.IgniteFactory.Instance().GetCache<long, UserCacheItem>(this.igniteFixture.Configuration.UserCacheName);
            this.userRoleCache = this.igniteFixture.IgniteFactory.Instance().GetCache<UserRoleKey, EmptyCacheItem>(this.igniteFixture.Configuration.UserRoleCacheName);
            this.userCategoryCache = this.igniteFixture.IgniteFactory.Instance().GetCache<UserCategoryKey, EmptyCacheItem>(this.igniteFixture.Configuration.UserCategoryCacheName);
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
            var categoryId = 1;
            long id = default;

            var user = new User(
                id,
                userName,
                normalizedName,
                passwordHash,
                new RoleBinding[] { new RoleBinding(roleId, id, null) },
                new CategoryBinding[] { new CategoryBinding(categoryId, id, null) });

            var userId = await this.repository.Add(user);

            var userCacheItem = await this.userCache.GetAsync(userId);
            Assert.NotNull(userCacheItem);
            Assert.Equal(userName, userCacheItem.UserName);
            Assert.Equal(normalizedName, userCacheItem.NormalizedUserName);
            Assert.Equal(passwordHash, userCacheItem.PasswordHash);

            var roleBinding = this.userRoleCache
                .AsCacheQueryable()
                .Where(r => r.Key.UserId == userId)
                .Select(r => r.Key)
                .FirstOrDefault();

            Assert.NotNull(roleBinding);
            Assert.Equal(roleId, roleBinding.RoleId);
            Assert.Equal(userId, roleBinding.UserId);

            var categoryBinding = this.userCategoryCache
                .AsCacheQueryable()
                .Where(r => r.Key.UserId == userId)
                .Select(r => r.Key)
                .FirstOrDefault();

            Assert.NotNull(categoryBinding);
            Assert.Equal(categoryId, categoryBinding.CategoryId);
            Assert.Equal(userId, categoryBinding.UserId);
        }

        [Fact]
        public async Task Update_ShouldUpdateUser()
        {
            var user = await this.InsertUser();
            var newPasswordHash = "testhash";
            var removedRoleId = 1;
            var newRoleId = 2;
            var newCategoryId = 2;

            user.PasswordHash = newPasswordHash;
            user.AddRole(newRoleId);
            user.RemoveRole(removedRoleId);
            user.SetCategories(new long[] { newCategoryId });

            await this.repository.Update(user);

            var userCacheItem = await this.userCache.GetAsync(user.Id);

            var roleBindings = this.userRoleCache
                .AsCacheQueryable()
                .Where(r => r.Key.UserId == user.Id)
                .Select(r => r.Key)
                .ToList();

            var categoryBindings = this.userCategoryCache
                .AsCacheQueryable()
                .Where(r => r.Key.UserId == user.Id)
                .Select(r => r.Key)
                .ToList();

            Assert.Equal(newPasswordHash, userCacheItem.PasswordHash);

            Assert.Single(roleBindings);
            Assert.Equal(newRoleId, roleBindings.First().RoleId);

            Assert.Single(categoryBindings);
            Assert.Equal(newCategoryId, categoryBindings.First().CategoryId);
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

            var categoryBindings = this.userCategoryCache
                .AsCacheQueryable()
                .Where(r => r.Key.UserId == user.Id)
                .Select(r => r.Key)
                .ToList();

            Assert.False(userCacheItem.Success);
            Assert.Empty(roleBindings);
            Assert.Empty(categoryBindings);
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
            Assert.NotEmpty(user.RoleBindings);
            Assert.NotEmpty(user.CategoryBindings);
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
            Assert.NotEmpty(user.RoleBindings);
            Assert.NotEmpty(user.CategoryBindings);
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
            var categoryId = 1;
            var userName = "test user";
            var normalizedUserName = "TEST USER";
            var passwordHash = "abc123";

            await this.userCache.PutAsync(userId, new UserCacheItem
            {
                UserName = userName,
                NormalizedUserName = normalizedUserName,
                PasswordHash = passwordHash,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "test user",
            });

            await this.userRoleCache.PutAsync(new UserRoleKey { UserId = userId, RoleId = roleId }, new EmptyCacheItem());
            await this.userCategoryCache.PutAsync(new UserCategoryKey { UserId = userId, CategoryId = categoryId }, new EmptyCacheItem());

            var user = new User(
                userId,
                userName,
                normalizedUserName,
                passwordHash,
                new RoleBinding[] { new RoleBinding(roleId, userId, null) },
                new CategoryBinding[] { new CategoryBinding(categoryId, userId, null) });

            return user;
        }
    }
}
