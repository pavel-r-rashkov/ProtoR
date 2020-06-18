namespace ProtoR.Infrastructure.DataAccess.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Core.DataStructures;
    using Apache.Ignite.Linq;
    using Microsoft.Extensions.Options;
    using ProtoR.Application;
    using ProtoR.Domain.ClientAggregate;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public class ClientRepository : BaseRepository, IClientRepository
    {
        private const char Separator = ',';
        private readonly ICache<long, ClientCacheItem> clientCache;
        private readonly ICache<ClientRoleKey, EmptyCacheItem> clientRoleCache;

        public ClientRepository(
            IIgniteFactory igniteFactory,
            IOptions<IgniteExternalConfiguration> configurationProvider,
            IUserProvider userProvider)
            : base(igniteFactory, configurationProvider, userProvider)
        {
            this.clientCache = this.Ignite.GetOrCreateCache<long, ClientCacheItem>(this.ConfigurationProvider.ClientCacheName);
            this.clientRoleCache = this.Ignite.GetOrCreateCache<ClientRoleKey, EmptyCacheItem>(this.ConfigurationProvider.ClientRoleCacheName);
        }

        public async Task<long> Add(Client client)
        {
            _ = client ?? throw new ArgumentNullException(nameof(client));

            IAtomicSequence clientIdGenerator = this.Ignite.GetAtomicSequence(
                $"{typeof(ClientCacheItem).Name.ToUpperInvariant()}{CacheConstants.IdSequenceSufix}",
                0,
                true);

            var id = clientIdGenerator.Increment();
            var clientItem = this.MapToCacheItem(client);

            var clientRoleItems = client.RoleBindings.Select(r => new KeyValuePair<ClientRoleKey, EmptyCacheItem>(
                new ClientRoleKey
                {
                    ClientId = id,
                    RoleId = r.RoleId,
                },
                new EmptyCacheItem()));

            await this.clientCache.PutIfAbsentAsync(id, clientItem);
            await this.clientRoleCache.PutAllAsync(clientRoleItems);

            return id;
        }

        public async Task Delete(long id)
        {
            await this.clientCache.RemoveAsync(id);

            var clientRoles = this.clientRoleCache
                .AsCacheQueryable()
                .Where(cr => cr.Key.ClientId == id)
                .Select(cr => cr.Key)
                .ToList();

            await this.clientRoleCache.RemoveAllAsync(clientRoles);
        }

        public Task<Client> GetByClientId(string clientId)
        {
            var clientCacheItem = this.clientCache
                .AsCacheQueryable()
                .Where(c => c.Value.ClientId.ToUpper() == clientId.ToUpper())
                .FirstOrDefault();

            if (clientCacheItem == null)
            {
                return Task.FromResult((Client)null);
            }

            var client = this.PopulateRoles(clientCacheItem.Key, clientCacheItem.Value);

            return Task.FromResult(client);
        }

        public async Task<Client> GetById(long id)
        {
            var result = await this.clientCache.TryGetAsync(id);

            if (!result.Success)
            {
                return null;
            }

            var clientCacheItem = result.Value;
            var client = this.PopulateRoles(id, clientCacheItem);

            return client;
        }

        public async Task Update(Client client)
        {
            _ = client ?? throw new ArgumentNullException(nameof(client));

            var clientItem = this.MapToCacheItem(client);

            var currentRoles = this.clientRoleCache
                .AsCacheQueryable()
                .Where(cr => cr.Key.ClientId == client.Id)
                .Select(cr => cr.Key)
                .ToList();

            var rolesToDelete = currentRoles.Where(cr => !client.RoleBindings
                .ToList()
                .Select(rb => rb.RoleId)
                .Contains(cr.RoleId));

            var rolesToAdd = client.RoleBindings
                .ToList()
                .Where(rb => !currentRoles
                    .Select(cr => cr.RoleId)
                    .Contains(rb.RoleId))
                .Select(rb => new KeyValuePair<ClientRoleKey, EmptyCacheItem>(
                    new ClientRoleKey { ClientId = client.Id, RoleId = rb.RoleId },
                    new EmptyCacheItem()));

            await this.clientCache.PutAsync(client.Id, clientItem);

            await this.clientRoleCache.PutAllAsync(rolesToAdd);
            await this.clientRoleCache.RemoveAllAsync(rolesToDelete);
        }

        private static Client MapToClient(
            long id,
            ClientCacheItem cacheItem,
            IReadOnlyCollection<RoleBinding> roles)
        {
            return new Client(
                id,
                cacheItem.ClientId,
                cacheItem.ClientName,
                cacheItem.Secret,
                cacheItem.GrantTypes.Split(Separator, StringSplitOptions.RemoveEmptyEntries),
                cacheItem.RedirectUris
                    .Split(Separator, StringSplitOptions.RemoveEmptyEntries)
                    .Select(u => new Uri(u))
                    .ToList(),
                cacheItem.PostLogoutRedirectUris
                    .Split(Separator, StringSplitOptions.RemoveEmptyEntries)
                    .Select(u => new Uri(u))
                    .ToList(),
                cacheItem.AllowedCorsOrigins
                    .Split(Separator, StringSplitOptions.RemoveEmptyEntries)
                    .ToList(),
                cacheItem.GroupRestrictions
                    .Split(Separator, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => new GroupRestriction(t))
                    .ToList(),
                roles);
        }

        private ClientCacheItem MapToCacheItem(Client client)
        {
            return new ClientCacheItem
            {
                ClientId = client.ClientId,
                ClientName = client.ClientName,
                Secret = client.Secret,
                GrantTypes = string.Join(Separator, client.GrantTypes),
                RedirectUris = string.Join(Separator, client.RedirectUris),
                PostLogoutRedirectUris = string.Join(Separator, client.PostLogoutRedirectUris),
                AllowedCorsOrigins = string.Join(Separator, client.AllowedCorsOrigins),
                GroupRestrictions = string.Join(Separator, client.GroupRestrictions.Select(g => g.Pattern)),
                CreatedBy = this.UserProvider.GetCurrentUserName(),
                CreatedOn = DateTime.UtcNow,
            };
        }

        private Client PopulateRoles(long id, ClientCacheItem clientCacheItem)
        {
            var roles = this.clientRoleCache
                .AsCacheQueryable()
                .Where(cr => cr.Key.ClientId == id)
                .Select(cr => cr.Key.RoleId)
                .ToList()
                .Select(roleId => new RoleBinding(roleId, null, id))
                .ToList();

            return MapToClient(
                id,
                clientCacheItem,
                roles);
        }
    }
}
