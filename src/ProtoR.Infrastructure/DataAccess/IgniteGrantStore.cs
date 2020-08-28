namespace ProtoR.Infrastructure.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Linq;
    using IdentityServer4.Models;
    using IdentityServer4.Stores;
    using Microsoft.Extensions.Options;
    using ProtoR.Application;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public class IgniteGrantStore : IPersistedGrantStore
    {
        private readonly IUserProvider userProvider;
        private readonly ICache<string, GrantCacheItem> grantCache;

        public IgniteGrantStore(
            IIgniteFactory igniteFactory,
            IOptions<IgniteExternalConfiguration> igniteConfiguration,
            IUserProvider userProvider)
        {
            var ignite = igniteFactory.Instance();
            this.userProvider = userProvider;
            var grantStoreCacheName = igniteConfiguration.Value.CacheNames.GrantStoreCacheName;
            this.grantCache = ignite.GetCache<string, GrantCacheItem>(grantStoreCacheName);
        }

        public Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var grantKeys = this.grantCache
                .AsCacheQueryable()
                .Where(g => g.Value.SubjectId == subjectId)
                .ToList();

            var results = grantKeys.Select(g => new PersistedGrant
            {
                Key = g.Key,
                ClientId = g.Value.ClientId,
                SubjectId = g.Value.SubjectId,
                Type = g.Value.Type,
                Expiration = g.Value.Expiration,
                CreationTime = g.Value.CreationTime,
                Data = g.Value.Data,
            });

            return Task.FromResult(results);
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            var cacheItem = await this.grantCache.GetAsync(key);

            return new PersistedGrant
            {
                Key = key,
                ClientId = cacheItem.ClientId,
                SubjectId = cacheItem.SubjectId,
                Type = cacheItem.Type,
                Expiration = cacheItem.Expiration,
                CreationTime = cacheItem.CreationTime,
                Data = cacheItem.Data,
            };
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            var grantKeys = this.grantCache
                .AsCacheQueryable()
                .Where(g => g.Value.SubjectId == subjectId
                    && g.Value.ClientId == clientId)
                .Select(g => g.Key)
                .ToList();

            await this.grantCache.RemoveAllAsync(grantKeys);
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            var grantKeys = this.grantCache
                .AsCacheQueryable()
                .Where(g => g.Value.SubjectId == subjectId
                    && g.Value.ClientId == clientId
                    && g.Value.Type == type)
                .Select(g => g.Key)
                .ToList();

            await this.grantCache.RemoveAllAsync(grantKeys);
        }

        public async Task RemoveAsync(string key)
        {
            await this.grantCache.RemoveAsync(key);
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            await this.grantCache.PutAsync(grant.Key, new GrantCacheItem
            {
                ClientId = grant.ClientId,
                CreationTime = grant.CreationTime,
                Data = grant.Data,
                Expiration = grant.Expiration,
                SubjectId = grant.SubjectId,
                Type = grant.Type,
                CreatedBy = this.userProvider.GetCurrentUserName(),
                CreatedOn = DateTime.UtcNow,
            });
        }
    }
}
