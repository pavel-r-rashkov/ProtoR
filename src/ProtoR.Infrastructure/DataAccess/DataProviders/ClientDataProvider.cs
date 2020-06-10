namespace ProtoR.Infrastructure.DataAccess.DataProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Linq;
    using Microsoft.Extensions.Options;
    using ProtoR.Application.Client;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public class ClientDataProvider : BaseDataProvider, IClientDataProvider
    {
        private const char Separator = ',';
        private readonly ICache<long, ClientCacheItem> clientCache;
        private readonly ICache<ClientRoleKey, EmptyCacheItem> clientRoleCache;
        private readonly ICache<ClientCategoryKey, EmptyCacheItem> clientCategoryCache;

        public ClientDataProvider(
            IIgniteFactory igniteFactory,
            IOptions<IgniteExternalConfiguration> configurationProvider)
            : base(igniteFactory, configurationProvider)
        {
            this.clientCache = this.Ignite.GetOrCreateCache<long, ClientCacheItem>(this.ConfigurationProvider.ClientCacheName);
            this.clientRoleCache = this.Ignite.GetOrCreateCache<ClientRoleKey, EmptyCacheItem>(this.ConfigurationProvider.ClientRoleCacheName);
            this.clientCategoryCache = this.Ignite.GetOrCreateCache<ClientCategoryKey, EmptyCacheItem>(this.ConfigurationProvider.ClientCategoryCacheName);
        }

        public async Task<ClientDto> GetById(long id)
        {
            var result = await this.clientCache.TryGetAsync(id);

            if (!result.Success)
            {
                return null;
            }

            var clientCacheItem = result.Value;

            var roleIds = this.clientRoleCache
                .AsCacheQueryable()
                .Where(cr => cr.Key.ClientId == id)
                .Select(cr => cr.Key.RoleId)
                .ToList();

            var categoryIds = this.clientCategoryCache
                .AsCacheQueryable()
                .Where(cr => cr.Key.ClientId == id)
                .Select(cr => cr.Key.CategoryId)
                .ToList();

            var client = this.MapFromCacheItem(id, clientCacheItem);
            client.RoleBindings = roleIds;
            client.CategoryBindings = categoryIds;

            return client;
        }

        public Task<IEnumerable<ClientDto>> GetClients()
        {
            var categoryGroups = this.clientCategoryCache
                .AsCacheQueryable()
                .Select(c => c.Key)
                .ToList()
                .GroupBy(c => c.ClientId)
                .ToDictionary(g => g.Key, g => g.Select(c => c.CategoryId));

            var roleGroups = this.clientRoleCache
                .AsCacheQueryable()
                .Select(c => c.Key)
                .ToList()
                .GroupBy(c => c.ClientId)
                .ToDictionary(g => g.Key, g => g.Select(c => c.RoleId));

            var clients = this.clientCache
                .AsCacheQueryable()
                .ToList()
                .Select(c =>
                {
                    var client = this.MapFromCacheItem(c.Key, c.Value);

                    if (categoryGroups.TryGetValue(c.Key, out var categories))
                    {
                        client.CategoryBindings = categories;
                    }

                    if (roleGroups.TryGetValue(c.Key, out var roles))
                    {
                        client.RoleBindings = roles;
                    }

                    return client;
                });

            return Task.FromResult(clients);
        }

        public Task<IEnumerable<string>> GetOrigins()
        {
            var origins = this.clientCache
                .AsCacheQueryable()
                .Select(c => c.Value.AllowedCorsOrigins)
                .ToList()
                .SelectMany(o => o.Split(Separator, StringSplitOptions.RemoveEmptyEntries));

            return Task.FromResult(origins);
        }

        private ClientDto MapFromCacheItem(long id, ClientCacheItem clientCacheItem)
        {
            return new ClientDto
            {
                Id = id,
                ClientId = clientCacheItem.ClientId,
                ClientName = clientCacheItem.ClientName,
                GrantTypes = clientCacheItem.GrantTypes.Split(Separator, StringSplitOptions.RemoveEmptyEntries),
                RedirectUris = clientCacheItem.RedirectUris.Split(Separator, StringSplitOptions.RemoveEmptyEntries),
                PostLogoutRedirectUris = clientCacheItem.PostLogoutRedirectUris.Split(Separator, StringSplitOptions.RemoveEmptyEntries),
                AllowedCorsOrigins = clientCacheItem.AllowedCorsOrigins.Split(Separator, StringSplitOptions.RemoveEmptyEntries),
                CreatedBy = clientCacheItem.CreatedBy,
                CreatedOn = clientCacheItem.CreatedOn,
            };
        }
    }
}
