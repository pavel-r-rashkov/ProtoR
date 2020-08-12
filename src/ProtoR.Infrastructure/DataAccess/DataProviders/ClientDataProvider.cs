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
    using ProtoR.Application.Common;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public class ClientDataProvider : BaseDataProvider, IClientDataProvider
    {
        private const char Separator = ',';
        private readonly ICache<long, ClientCacheItem> clientCache;
        private readonly ICache<ClientRoleKey, EmptyCacheItem> clientRoleCache;

        public ClientDataProvider(
            IIgniteFactory igniteFactory,
            IOptions<IgniteExternalConfiguration> configurationProvider)
            : base(igniteFactory, configurationProvider)
        {
            this.clientCache = this.Ignite.GetOrCreateCache<long, ClientCacheItem>(this.ConfigurationProvider.ClientCacheName);
            this.clientRoleCache = this.Ignite.GetOrCreateCache<ClientRoleKey, EmptyCacheItem>(this.ConfigurationProvider.ClientRoleCacheName);
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

            var client = this.MapFromCacheItem(id, clientCacheItem);
            client.RoleBindings = roleIds;

            return client;
        }

        public Task<PagedResult<ClientDto>> GetClients(
            IEnumerable<Filter> filters,
            IEnumerable<SortOrder> sortOrders,
            Pagination pagination)
        {
            var roleGroups = this.clientRoleCache
                .AsCacheQueryable()
                .Select(c => c.Key)
                .ToList()
                .GroupBy(c => c.ClientId)
                .ToDictionary(g => g.Key, g => g.Select(c => c.RoleId));

            var clients = this.clientCache
                .AsCacheQueryable()
                .Select(c => new
                {
                    Id = c.Key,
                    c.Value.ClientId,
                    c.Value.ClientName,
                    c.Value.IsActive,
                    c.Value.GrantTypes,
                    c.Value.RedirectUris,
                    c.Value.PostLogoutRedirectUris,
                    c.Value.AllowedCorsOrigins,
                    c.Value.GroupRestrictions,
                    c.Value.CreatedBy,
                    c.Value.CreatedOn,
                })
                .Filter(filters);

            var totalCount = clients
                .Select(c => c.Id)
                .Count();

            var results = clients
                .Sort(sortOrders)
                .Page(pagination)
                .ToList()
                .Select(c =>
                {
                    var client = new ClientDto
                    {
                        Id = c.Id,
                        ClientId = c.ClientId,
                        ClientName = c.ClientName,
                        IsActive = c.IsActive,
                        GrantTypes = c.GrantTypes.Split(Separator, StringSplitOptions.RemoveEmptyEntries),
                        RedirectUris = c.RedirectUris.Split(Separator, StringSplitOptions.RemoveEmptyEntries),
                        PostLogoutRedirectUris = c.PostLogoutRedirectUris.Split(Separator, StringSplitOptions.RemoveEmptyEntries),
                        AllowedCorsOrigins = c.AllowedCorsOrigins.Split(Separator, StringSplitOptions.RemoveEmptyEntries),
                        GroupRestrictions = c.GroupRestrictions.Split(Separator, StringSplitOptions.RemoveEmptyEntries),
                        CreatedBy = c.CreatedBy,
                        CreatedOn = c.CreatedOn,
                    };

                    if (roleGroups.TryGetValue(c.Id, out var roles))
                    {
                        client.RoleBindings = roles;
                    }

                    return client;
                });

            return Task.FromResult(new PagedResult<ClientDto>(totalCount, results));
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
                IsActive = clientCacheItem.IsActive,
                GrantTypes = clientCacheItem.GrantTypes.Split(Separator, StringSplitOptions.RemoveEmptyEntries),
                RedirectUris = clientCacheItem.RedirectUris.Split(Separator, StringSplitOptions.RemoveEmptyEntries),
                PostLogoutRedirectUris = clientCacheItem.PostLogoutRedirectUris.Split(Separator, StringSplitOptions.RemoveEmptyEntries),
                AllowedCorsOrigins = clientCacheItem.AllowedCorsOrigins.Split(Separator, StringSplitOptions.RemoveEmptyEntries),
                GroupRestrictions = clientCacheItem.GroupRestrictions.Split(Separator, StringSplitOptions.RemoveEmptyEntries),
                CreatedBy = clientCacheItem.CreatedBy,
                CreatedOn = clientCacheItem.CreatedOn,
            };
        }
    }
}
