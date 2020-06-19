namespace ProtoR.Infrastructure.DataAccess.DataProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Linq;
    using Microsoft.Extensions.Options;
    using ProtoR.Application.User;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public class UserDataProvider : BaseDataProvider, IUserDataProvider
    {
        private const char Separator = ',';
        private readonly ICache<long, UserCacheItem> userCache;
        private readonly ICache<UserRoleKey, EmptyCacheItem> userRoleCache;

        public UserDataProvider(
            IIgniteFactory igniteFactory,
            IOptions<IgniteExternalConfiguration> configurationProvider)
            : base(igniteFactory, configurationProvider)
        {
            this.userCache = this.Ignite.GetOrCreateCache<long, UserCacheItem>(this.ConfigurationProvider.UserCacheName);
            this.userRoleCache = this.Ignite.GetOrCreateCache<UserRoleKey, EmptyCacheItem>(this.ConfigurationProvider.UserRoleCacheName);
        }

        public async Task<UserDto> GetById(long id)
        {
            var result = await this.userCache.TryGetAsync(id);

            if (!result.Success)
            {
                return null;
            }

            var userCacheItem = result.Value;

            var roleIds = this.userRoleCache
                .AsCacheQueryable()
                .Where(cr => cr.Key.UserId == id)
                .Select(cr => cr.Key.RoleId)
                .ToList();

            var user = this.MapFromCacheItem(id, userCacheItem);
            user.RoleBindings = roleIds;

            return user;
        }

        public Task<IEnumerable<UserDto>> GetUsers()
        {
            var roleGroups = this.userRoleCache
                .AsCacheQueryable()
                .Select(c => c.Key)
                .ToList()
                .GroupBy(c => c.UserId)
                .ToDictionary(g => g.Key, g => g.Select(c => c.RoleId));

            var users = this.userCache
                .AsCacheQueryable()
                .ToList()
                .Select(c =>
                {
                    var user = this.MapFromCacheItem(c.Key, c.Value);

                    if (roleGroups.TryGetValue(c.Key, out var roles))
                    {
                        user.RoleBindings = roles;
                    }

                    return user;
                });

            return Task.FromResult(users);
        }

        private UserDto MapFromCacheItem(long id, UserCacheItem userCacheItem)
        {
            return new UserDto
            {
                Id = id,
                UserName = userCacheItem.UserName,
                IsActive = userCacheItem.IsActive,
                GroupRestrictions = userCacheItem.GroupRestrictions
                    .Split(Separator, StringSplitOptions.RemoveEmptyEntries),
                CreatedBy = userCacheItem.CreatedBy,
                CreatedOn = userCacheItem.CreatedOn,
            };
        }
    }
}
