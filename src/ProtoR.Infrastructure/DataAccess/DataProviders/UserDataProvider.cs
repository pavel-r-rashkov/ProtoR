namespace ProtoR.Infrastructure.DataAccess.DataProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Linq;
    using Microsoft.Extensions.Options;
    using ProtoR.Application.Common;
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
            this.userCache = this.Ignite.GetOrCreateCache<long, UserCacheItem>(this.ConfigurationProvider.CacheNames.UserCacheName);
            this.userRoleCache = this.Ignite.GetOrCreateCache<UserRoleKey, EmptyCacheItem>(this.ConfigurationProvider.CacheNames.UserRoleCacheName);
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

        public Task<PagedResult<UserDto>> GetUsers(
            IEnumerable<Filter> filters,
            IEnumerable<SortOrder> sortOrders,
            Pagination pagination)
        {
            var roleGroups = this.userRoleCache
                .AsCacheQueryable()
                .Select(c => c.Key)
                .ToList()
                .GroupBy(c => c.UserId)
                .ToDictionary(g => g.Key, g => g.Select(c => c.RoleId));

            var users = this.userCache
                .AsCacheQueryable()
                .Select(u => new
                {
                    Id = u.Key,
                    u.Value.UserName,
                    u.Value.IsActive,
                    u.Value.GroupRestrictions,
                    u.Value.CreatedBy,
                    u.Value.CreatedOn,
                })
                .Filter(filters);

            var totalCount = users
                .Select(u => u.Id)
                .Count();

            var results = users
                .Sort(sortOrders)
                .Page(pagination)
                .ToList()
                .Select(c =>
                {
                    var user = new UserDto
                    {
                        Id = c.Id,
                        UserName = c.UserName,
                        IsActive = c.IsActive,
                        GroupRestrictions = c.GroupRestrictions
                            .Split(Separator, StringSplitOptions.RemoveEmptyEntries),
                        CreatedBy = c.CreatedBy,
                        CreatedOn = c.CreatedOn,
                    };

                    if (roleGroups.TryGetValue(c.Id, out var roles))
                    {
                        user.RoleBindings = roles;
                    }

                    return user;
                });

            return Task.FromResult(new PagedResult<UserDto>(totalCount, results));
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
