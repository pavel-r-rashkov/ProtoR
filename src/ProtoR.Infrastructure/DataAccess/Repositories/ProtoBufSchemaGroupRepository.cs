namespace ProtoR.Infrastructure.DataAccess.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Linq;
    using ProtoR.Application;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Infrastructure.DataAccess.CacheItems;
    using Version = ProtoR.Domain.SchemaGroupAggregate.Schemas.Version;

    public class ProtoBufSchemaGroupRepository : BaseRepository, IProtoBufSchemaGroupRepository
    {
        private readonly string schemaCacheName;
        private readonly string schemaGroupCacheName;
        private readonly string configurationCacheName;
        private readonly string ruleConfigurationCacheName;

        public ProtoBufSchemaGroupRepository(
            IIgniteFactory igniteFactory,
            IUserProvider userProvider,
            IIgniteConfiguration configurationProvider)
            : base(igniteFactory, configurationProvider, userProvider)
        {
            this.schemaCacheName = this.ConfigurationProvider.SchemaCacheName;
            this.schemaGroupCacheName = this.ConfigurationProvider.SchemaGroupCacheName;
            this.configurationCacheName = this.ConfigurationProvider.ConfigurationCacheName;
            this.ruleConfigurationCacheName = this.ConfigurationProvider.RuleConfigurationCacheName;
        }

        public async Task<long> Add(ProtoBufSchemaGroup schemaGroup)
        {
            _ = schemaGroup ?? throw new ArgumentNullException(nameof(schemaGroup));

            var schemaGroupCacheItem = new SchemaGroupCacheItem
            {
                Name = schemaGroup.Name,
                CategoryId = schemaGroup.CategoryId,
                CreatedBy = this.UserProvider.GetCurrentUserName(),
                CreatedOn = DateTime.UtcNow,
            };

            var cache = this.Ignite.GetCache<long, SchemaGroupCacheItem>(this.schemaGroupCacheName);
            var idGenerator = this.Ignite.GetAtomicSequence(
                $"{typeof(SchemaGroupCacheItem).Name.ToUpperInvariant()}{CacheConstants.IdSequenceSufix}",
                0,
                true);
            long id = idGenerator.Increment();
            await cache.PutIfAbsentAsync(id, schemaGroupCacheItem);
            await this.AddNewSchemas(schemaGroup, id);

            return id;
        }

        public Task<ProtoBufSchemaGroup> GetByName(string name)
        {
            var schemaGroupCache = this.Ignite.GetCache<long, SchemaGroupCacheItem>(this.schemaGroupCacheName);
            var schemaGroupCacheItem = schemaGroupCache
                .AsCacheQueryable()
                .FirstOrDefault(c => c.Value.Name.ToUpper() == name.ToUpper());

            if (schemaGroupCacheItem == null)
            {
                return Task.FromResult((ProtoBufSchemaGroup)null);
            }

            var schemaCache = this.Ignite.GetCache<long, SchemaCacheItem>(this.schemaCacheName);
            var schemaCacheItems = schemaCache
                .AsCacheQueryable()
                .Where(c => c.Value.SchemaGroupId == schemaGroupCacheItem.Key)
                .ToList()
                .Select(c => new ProtoBufSchema(c.Key, new Version(c.Value.Version), c.Value.Contents));

            return Task.FromResult(new ProtoBufSchemaGroup(
                schemaGroupCacheItem.Key,
                schemaGroupCacheItem.Value.Name,
                schemaGroupCacheItem.Value.CategoryId,
                schemaCacheItems));
        }

        public async Task Update(ProtoBufSchemaGroup schemaGroup)
        {
            _ = schemaGroup ?? throw new ArgumentNullException(nameof(schemaGroup));

            var schemaGroupCache = this.Ignite.GetCache<long, SchemaGroupCacheItem>(this.schemaGroupCacheName);
            var schemaGroupCacheItem = await schemaGroupCache.GetAsync(schemaGroup.Id);
            schemaGroupCacheItem.CategoryId = schemaGroup.CategoryId;
            await schemaGroupCache.PutAsync(schemaGroup.Id, schemaGroupCacheItem);
            await this.AddNewSchemas(schemaGroup, schemaGroup.Id);
        }

        public async Task Delete(ProtoBufSchemaGroup schemaGroup)
        {
            _ = schemaGroup ?? throw new ArgumentNullException(nameof(schemaGroup));

            var cache = this.Ignite.GetCache<long, SchemaGroupCacheItem>(this.schemaGroupCacheName);
            await cache.RemoveAsync(schemaGroup.Id);

            var schemaCache = this.Ignite.GetCache<long, SchemaCacheItem>(this.schemaCacheName);
            await schemaCache.RemoveAllAsync(schemaGroup.Schemas.Select(s => s.Id));

            var configurationCache = this.Ignite.GetCache<long, ConfigurationCacheItem>(this.configurationCacheName);
            var configurationId = configurationCache
                .AsCacheQueryable()
                .First(c => c.Value.SchemaGroupId == schemaGroup.Id)
                .Key;

            await configurationCache.RemoveAsync(configurationId);

            var ruleConfigurationCache = this.Ignite.GetCache<long, RuleConfigurationCacheItem>(this.ruleConfigurationCacheName);
            var ruleConfigurationIds = ruleConfigurationCache
                .AsCacheQueryable()
                .Where(r => r.Value.ConfigurationId == configurationId)
                .Select(r => r.Key)
                .ToList();

            await ruleConfigurationCache.RemoveAllAsync(ruleConfigurationIds);
        }

        private async Task AddNewSchemas(ProtoBufSchemaGroup schemaGroup, long groupId)
        {
            var schemaCache = this.Ignite.GetCache<long, SchemaCacheItem>(this.schemaCacheName);
            var idGenerator = this.Ignite.GetAtomicSequence(
                $"{typeof(SchemaCacheItem).Name.ToUpperInvariant()}{CacheConstants.IdSequenceSufix}",
                0,
                true);

            var newSchemas = schemaGroup.Schemas
                .Where(s => s.Id == default)
                .Select(s => new KeyValuePair<long, SchemaCacheItem>(
                    idGenerator.Increment(),
                    new SchemaCacheItem
                    {
                        SchemaGroupId = groupId,
                        Version = s.Version.VersionNumber,
                        Contents = s.Contents,
                        CreatedBy = this.UserProvider.GetCurrentUserName(),
                        CreatedOn = DateTime.UtcNow,
                    }));

            await schemaCache.PutAllAsync(newSchemas);
        }
    }
}
