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
    using ProtoR.Domain.CategoryAggregate;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Domain.UserAggregate;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly ICache<long, UserCacheItem> userCache;
        private readonly ICache<UserRoleKey, EmptyCacheItem> userRoleCache;
        private readonly ICache<UserCategoryKey, EmptyCacheItem> userCategoryCache;

        public UserRepository(
            IIgniteFactory igniteFactory,
            IIgniteConfiguration configurationProvider,
            IUserProvider userProvider)
            : base(igniteFactory, configurationProvider, userProvider)
        {
            this.userCache = this.Ignite.GetOrCreateCache<long, UserCacheItem>(this.ConfigurationProvider.UserCacheName);
            this.userRoleCache = this.Ignite.GetOrCreateCache<UserRoleKey, EmptyCacheItem>(this.ConfigurationProvider.UserRoleCacheName);
            this.userCategoryCache = this.Ignite.GetOrCreateCache<UserCategoryKey, EmptyCacheItem>(this.ConfigurationProvider.UserCategoryCacheName);
        }

        public async Task<long> Add(User user)
        {
            _ = user ?? throw new ArgumentNullException(nameof(user));

            IAtomicSequence userIdGenerator = this.Ignite.GetAtomicSequence(
                $"{typeof(UserCacheItem).Name.ToUpperInvariant()}{CacheConstants.IdSequenceSufix}",
                0,
                true);

            var id = userIdGenerator.Increment();

            var userItem = new UserCacheItem();
            this.MapToUserCacheItem(user, userItem);
            userItem.CreatedBy = this.UserProvider.GetCurrentUserName();
            userItem.CreatedOn = DateTime.UtcNow;

            var userRoleItems = user.RoleBindings.Select(r => new KeyValuePair<UserRoleKey, EmptyCacheItem>(
                new UserRoleKey
                {
                    UserId = id,
                    RoleId = r.RoleId,
                },
                new EmptyCacheItem()));

            var userCategoryItems = user.CategoryBindings.Select(c => new KeyValuePair<UserCategoryKey, EmptyCacheItem>(
                new UserCategoryKey
                {
                    UserId = id,
                    CategoryId = c.CategoryId,
                },
                new EmptyCacheItem()));

            await this.userCache.PutIfAbsentAsync(id, userItem);
            await this.userRoleCache.PutAllAsync(userRoleItems);
            await this.userCategoryCache.PutAllAsync(userCategoryItems);

            return id;
        }

        public async Task Delete(long id)
        {
            await this.userCache.RemoveAsync(id);

            var userRoles = this.userRoleCache
                .AsCacheQueryable()
                .Where(cr => cr.Key.UserId == id)
                .Select(cr => cr.Key)
                .ToList();

            await this.userRoleCache.RemoveAllAsync(userRoles);

            var userCategories = this.userCategoryCache
                .AsCacheQueryable()
                .Where(cr => cr.Key.UserId == id)
                .Select(cr => cr.Key)
                .ToList();

            await this.userCategoryCache.RemoveAllAsync(userCategories);
        }

        public async Task<User> GetById(long id)
        {
            var userCacheItem = await this.userCache.GetAsync(id);

            if (userCacheItem == null)
            {
                return null;
            }

            var user = this.PopulateRolesAndCategories(id, userCacheItem);

            return user;
        }

        public Task<User> GetByName(string normalizedName)
        {
            var userCacheItem = this.userCache
                .AsCacheQueryable()
                .Where(c => c.Value.NormalizedUserName == normalizedName)
                .FirstOrDefault();

            if (userCacheItem == null)
            {
                return Task.FromResult((User)null);
            }

            var user = this.PopulateRolesAndCategories(userCacheItem.Key, userCacheItem.Value);

            return Task.FromResult(user);
        }

        public async Task<IEnumerable<User>> GetUsersInRole(long roleId)
        {
            var userIds = this.userRoleCache
                .AsCacheQueryable()
                .Where(r => r.Key.RoleId == roleId)
                .Select(r => r.Key.UserId)
                .ToList();

            var userCacheItems = await this.userCache.GetAllAsync(userIds);

            return userCacheItems.Select(u => new User(
                u.Key,
                u.Value.UserName,
                u.Value.NormalizedUserName,
                u.Value.PasswordHash,
                new List<RoleBinding>(),
                new List<CategoryBinding>()));
        }

        public async Task Update(User user)
        {
            _ = user ?? throw new ArgumentNullException(nameof(user));

            var userCacheItem = await this.userCache.GetAsync(user.Id);
            this.MapToUserCacheItem(user, userCacheItem);

            var currentRoles = this.userRoleCache
                .AsCacheQueryable()
                .Where(cr => cr.Key.UserId == user.Id)
                .Select(cr => cr.Key)
                .ToList();

            var rolesToDelete = currentRoles.Where(cr => !user.RoleBindings
                .ToList()
                .Select(rb => rb.RoleId)
                .Contains(cr.RoleId));

            var rolesToAdd = user.RoleBindings
                .ToList()
                .Where(rb => !currentRoles
                    .Select(cr => cr.RoleId)
                    .Contains(rb.RoleId))
                .Select(rb => new KeyValuePair<UserRoleKey, EmptyCacheItem>(
                    new UserRoleKey { UserId = user.Id, RoleId = rb.RoleId },
                    new EmptyCacheItem()));

            var currentCategories = this.userCategoryCache
                .AsCacheQueryable()
                .Where(cr => cr.Key.UserId == user.Id)
                .Select(cr => cr.Key)
                .ToList();

            var categoriesToDelete = currentCategories.Where(c => !user.CategoryBindings
                .ToList()
                .Select(cb => cb.CategoryId)
                .Contains(c.CategoryId));

            var categoriesToAdd = user.CategoryBindings
                .ToList()
                .Where(cb => !currentCategories
                    .Select(cr => cr.CategoryId)
                    .Contains(cb.CategoryId))
                .Select(cb => new KeyValuePair<UserCategoryKey, EmptyCacheItem>(
                    new UserCategoryKey { UserId = user.Id, CategoryId = cb.CategoryId },
                    new EmptyCacheItem()));

            await this.userCache.PutAsync(user.Id, userCacheItem);

            await this.userRoleCache.PutAllAsync(rolesToAdd);
            await this.userRoleCache.RemoveAllAsync(rolesToDelete);

            await this.userCategoryCache.PutAllAsync(categoriesToAdd);
            await this.userCategoryCache.RemoveAllAsync(categoriesToDelete);
        }

        private void MapToUserCacheItem(User user, UserCacheItem cacheItem)
        {
            cacheItem.UserName = user.UserName;
            cacheItem.NormalizedUserName = user.NormalizedUserName;
            cacheItem.PasswordHash = user.PasswordHash;
        }

        private User PopulateRolesAndCategories(long id, UserCacheItem userCacheItem)
        {
            var roles = this.userRoleCache
                .AsCacheQueryable()
                .Where(cr => cr.Key.UserId == id)
                .Select(cr => cr.Key.RoleId)
                .ToList()
                .Select(roleId => new RoleBinding(roleId, null, id))
                .ToList();

            var categories = this.userCategoryCache
                .AsCacheQueryable()
                .Where(cr => cr.Key.UserId == id)
                .Select(cr => cr.Key.CategoryId)
                .ToList()
                .Select(categoryId => new CategoryBinding(categoryId, null, id))
                .ToList();

            return new User(
                id,
                userCacheItem.UserName,
                userCacheItem.NormalizedUserName,
                userCacheItem.PasswordHash,
                roles,
                categories);
        }
    }
}
