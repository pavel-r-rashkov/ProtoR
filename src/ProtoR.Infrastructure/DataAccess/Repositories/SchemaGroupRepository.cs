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
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Infrastructure.DataAccess.CacheItems;
    using Version = ProtoR.Domain.SchemaGroupAggregate.Schemas.Version;

    public class SchemaGroupRepository : ISchemaGroupRepository<ProtoBufSchema, FileDescriptorSet>
    {
        private readonly IIgnite ignite;
        private readonly IUserProvider userProvider;
        private readonly string schemaCacheName;
        private readonly string schemaGroupCacheName;

        public SchemaGroupRepository(
            IIgnite ignite,
            IUserProvider userProvider,
            IIgniteConfigurationProvider configurationProvider)
        {
            this.ignite = ignite;
            this.userProvider = userProvider;
            this.schemaCacheName = configurationProvider.SchemaCacheName;
            this.schemaGroupCacheName = configurationProvider.SchemaGroupCacheName;
        }

        public async Task<long> Add(SchemaGroup<ProtoBufSchema, FileDescriptorSet> schemaGroup)
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

        public Task<SchemaGroup<ProtoBufSchema, FileDescriptorSet>> GetByName(string name)
        {
            ICache<long, SchemaGroupCacheItem> schemaGroupCache = this.ignite.GetCache<long, SchemaGroupCacheItem>(this.schemaGroupCacheName);
            ICacheEntry<long, SchemaGroupCacheItem> schemaCacheItem = schemaGroupCache
                .AsCacheQueryable()
                .FirstOrDefault(c => c.Value.Name.ToUpper() == name.ToUpper());

            ICache<long, SchemaCacheItem> schemaCache = this.ignite.GetCache<long, SchemaCacheItem>(this.schemaCacheName);

            var t = schemaCache
                .AsCacheQueryable()
                .Where(c => c.Value.SchemaGroupId == schemaCacheItem.Key)
                .ToList();

            IEnumerable<ProtoBufSchema> schemaCacheItems = schemaCache
                .AsCacheQueryable()
                .Where(c => c.Value.SchemaGroupId == schemaCacheItem.Key)
                .ToList()
                .Select(c => new ProtoBufSchema(c.Key, new Version(c.Value.Version), c.Value.Contents));

            return Task.FromResult(new SchemaGroup<ProtoBufSchema, FileDescriptorSet>(
                schemaCacheItem.Key,
                schemaCacheItem.Value.Name,
                schemaCacheItems,
                RuleFactory.GetProtoBufRules()));
        }

        public async Task Update(SchemaGroup<ProtoBufSchema, FileDescriptorSet> schemaGroup)
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
