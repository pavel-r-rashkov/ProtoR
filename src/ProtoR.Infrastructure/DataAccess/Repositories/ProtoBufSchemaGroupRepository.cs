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
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Infrastructure.DataAccess.CacheItems;
    using Version = ProtoR.Domain.SchemaGroupAggregate.Schemas.Version;

    public class ProtoBufSchemaGroupRepository : IProtoBufSchemaGroupRepository
    {
        private readonly IIgnite ignite;
        private readonly IUserProvider userProvider;
        private readonly string schemaCacheName;
        private readonly string schemaGroupCacheName;

        public ProtoBufSchemaGroupRepository(
            IIgniteFactory igniteFactory,
            IUserProvider userProvider,
            IIgniteConfiguration configuration)
        {
            this.ignite = igniteFactory.Instance();
            this.userProvider = userProvider;
            this.schemaCacheName = configuration.SchemaCacheName;
            this.schemaGroupCacheName = configuration.SchemaGroupCacheName;
        }

        public async Task<long> Add(ProtoBufSchemaGroup schemaGroup)
        {
            var schemaGroupCacheItem = new SchemaGroupCacheItem
            {
                Name = schemaGroup.Name,
                CreatedBy = this.userProvider.GetCurrentUserName(),
                CreatedOn = DateTime.UtcNow,
            };

            ICache<long, SchemaGroupCacheItem> cache = this.ignite.GetCache<long, SchemaGroupCacheItem>(this.schemaGroupCacheName);
            IAtomicSequence idGenerator = this.ignite.GetAtomicSequence(
                $"{typeof(SchemaGroupCacheItem).Name.ToUpperInvariant()}{CacheConstants.IdSequenceSufix}",
                0,
                true);
            long id = idGenerator.Increment();
            await cache.PutIfAbsentAsync(id, schemaGroupCacheItem);

            return id;
        }

        public Task<ProtoBufSchemaGroup> GetByName(string name)
        {
            ICache<long, SchemaGroupCacheItem> schemaGroupCache = this.ignite.GetCache<long, SchemaGroupCacheItem>(this.schemaGroupCacheName);
            ICacheEntry<long, SchemaGroupCacheItem> schemaCacheItem = schemaGroupCache
                .AsCacheQueryable()
                .FirstOrDefault(c => c.Value.Name.ToUpper() == name.ToUpper());

            ICache<long, SchemaCacheItem> schemaCache = this.ignite.GetCache<long, SchemaCacheItem>(this.schemaCacheName);
            IEnumerable<ProtoBufSchema> schemaCacheItems = schemaCache
                .AsCacheQueryable()
                .Where(c => c.Value.SchemaGroupId == schemaCacheItem.Key)
                .ToList()
                .Select(c => new ProtoBufSchema(c.Key, new Version(c.Value.Version), c.Value.Contents));

            return Task.FromResult(new ProtoBufSchemaGroup(
                schemaCacheItem.Key,
                schemaCacheItem.Value.Name,
                schemaCacheItems));
        }

        public async Task Update(ProtoBufSchemaGroup schemaGroup)
        {
            ICache<long, SchemaCacheItem> schemaCache = this.ignite.GetCache<long, SchemaCacheItem>(this.schemaCacheName);
            IAtomicSequence idGenerator = this.ignite.GetAtomicSequence(
                $"{typeof(SchemaCacheItem).Name.ToUpperInvariant()}{CacheConstants.IdSequenceSufix}",
                0,
                true);

            IEnumerable<KeyValuePair<long, SchemaCacheItem>> newSchemas = schemaGroup.Schemas
                .Where(s => s.Id == default)
                .Select(s => new KeyValuePair<long, SchemaCacheItem>(
                    idGenerator.Increment(),
                    new SchemaCacheItem
                    {
                        SchemaGroupId = schemaGroup.Id,
                        Version = s.Version.VersionNumber,
                        Contents = s.Contents,
                        CreatedBy = this.userProvider.GetCurrentUserName(),
                        CreatedOn = DateTime.UtcNow,
                    }));

            await schemaCache.PutAllAsync(newSchemas);
        }
    }
}
