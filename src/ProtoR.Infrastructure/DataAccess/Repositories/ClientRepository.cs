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
    using ProtoR.Domain.ClientAggregate;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public class ClientRepository : BaseRepository, IClientRepository
    {
        private const char Separator = ',';
        private readonly ICache<long, ClientCacheItem> clientCache;
        private readonly ICache<ClientRoleKey, EmptyCacheItem> clientRoleCache;
        private readonly ICache<ClientCategoryKey, EmptyCacheItem> clientCategoryCache;

        public ClientRepository(
            IIgniteFactory igniteFactory,
            IIgniteConfiguration configurationProvider,
            IUserProvider userProvider)
            : base(igniteFactory, configurationProvider, userProvider)
        {
            this.clientCache = this.Ignite.GetOrCreateCache<long, ClientCacheItem>(configurationProvider.ClientCacheName);
            this.clientRoleCache = this.Ignite.GetOrCreateCache<ClientRoleKey, EmptyCacheItem>(configurationProvider.ClientRoleCacheName);
            this.clientCategoryCache = this.Ignite.GetOrCreateCache<ClientCategoryKey, EmptyCacheItem>(configurationProvider.ClientCategoryCacheName);
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

            var clientCategoryItems = client.CategoryBindings.Select(c => new KeyValuePair<ClientCategoryKey, EmptyCacheItem>(
                new ClientCategoryKey
                {
                    ClientId = id,
                    CategoryId = c.CategoryId,
                },
                new EmptyCacheItem()));

            await this.clientCache.PutIfAbsentAsync(id, clientItem);
            await this.clientRoleCache.PutAllAsync(clientRoleItems);
            await this.clientCategoryCache.PutAllAsync(clientCategoryItems);

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

            var clientCategories = this.clientCategoryCache
                .AsCacheQueryable()
                .Where(cr => cr.Key.ClientId == id)
                .Select(cr => cr.Key)
                .ToList();

            await this.clientCategoryCache.RemoveAllAsync(clientCategories);
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

            var client = this.PopulateRolesAndCategories(clientCacheItem.Key, clientCacheItem.Value);

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
            var client = this.PopulateRolesAndCategories(id, clientCacheItem);

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

            var currentCategories = this.clientCategoryCache
                .AsCacheQueryable()
                .Where(cr => cr.Key.ClientId == client.Id)
                .Select(cr => cr.Key)
                .ToList();

            var categoriesToDelete = currentCategories.Where(c => !client.CategoryBindings
                .ToList()
                .Select(cb => cb.CategoryId)
                .Contains(c.CategoryId));

            var categoriesToAdd = client.CategoryBindings
                .ToList()
                .Where(cb => !currentCategories
                    .Select(cr => cr.CategoryId)
                    .Contains(cb.CategoryId))
                .Select(cb => new KeyValuePair<ClientCategoryKey, EmptyCacheItem>(
                    new ClientCategoryKey { ClientId = client.Id, CategoryId = cb.CategoryId },
                    new EmptyCacheItem()));

            await this.clientCache.PutAsync(client.Id, clientItem);

            await this.clientRoleCache.PutAllAsync(rolesToAdd);
            await this.clientRoleCache.RemoveAllAsync(rolesToDelete);

            await this.clientCategoryCache.PutAllAsync(categoriesToAdd);
            await this.clientCategoryCache.RemoveAllAsync(categoriesToDelete);
        }

        private static Client MapToClient(
            long id,
            ClientCacheItem cacheItem,
            IReadOnlyCollection<RoleBinding> roles,
            IReadOnlyCollection<CategoryBinding> categories)
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
                roles,
                categories);
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
                CreatedBy = this.UserProvider.GetCurrentUserName(),
                CreatedOn = DateTime.UtcNow,
            };
        }

        private Client PopulateRolesAndCategories(long id, ClientCacheItem clientCacheItem)
        {
            var roles = this.clientRoleCache
                .AsCacheQueryable()
                .Where(cr => cr.Key.ClientId == id)
                .Select(cr => cr.Key.RoleId)
                .ToList()
                .Select(roleId => new RoleBinding(roleId, null, id))
                .ToList();

            var categories = this.clientCategoryCache
                .AsCacheQueryable()
                .Where(cr => cr.Key.ClientId == id)
                .Select(cr => cr.Key.CategoryId)
                .ToList()
                .Select(categoryId => new CategoryBinding(categoryId, null, id))
                .ToList();

            return MapToClient(
                id,
                clientCacheItem,
                roles,
                categories);
        }
    }
}
