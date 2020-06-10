namespace ProtoR.Infrastructure.DataAccess.DataProviders
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Linq;
    using ProtoR.Application.Schema;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public class SchemaDataProvider : BaseDataProvider, ISchemaDataProvider
    {
        private readonly string groupCacheName;
        private readonly string schemaCacheName;

        public SchemaDataProvider(
            IIgniteFactory igniteFactory,
            IIgniteConfiguration configurationProvider)
            : base(igniteFactory, configurationProvider)
        {
            this.groupCacheName = this.ConfigurationProvider.SchemaGroupCacheName;
            this.schemaCacheName = this.ConfigurationProvider.SchemaCacheName;
        }

        public async Task<SchemaDto> GetByVersion(string groupName, int version)
        {
            var groupId = await this.GetGroupId(groupName);
            var schemaCache = this.Ignite.GetCache<long, SchemaCacheItem>(this.schemaCacheName);
            var schema = schemaCache
                .AsCacheQueryable()
                .Where(c => c.Value.SchemaGroupId == groupId && c.Value.Version == version)
                .Select(c => new
                {
                    Id = c.Key,
                    c.Value.Version,
                    c.Value.Contents,
                    c.Value.CreatedBy,
                    c.Value.CreatedOn,
                })
                .FirstOrDefault();

            if (schema == null)
            {
                return null;
            }

            return new SchemaDto
            {
                Id = schema.Id,
                Version = schema.Version,
                Contents = schema.Contents,
                CreatedBy = schema.CreatedBy,
                CreatedOn = schema.CreatedOn,
            };
        }

        public async Task<IEnumerable<SchemaDto>> GetGroupSchemas(string groupName)
        {
            long groupId = await this.GetGroupId(groupName);
            ICache<long, SchemaCacheItem> schemaCache = this.Ignite.GetCache<long, SchemaCacheItem>(this.schemaCacheName);
            var schemas = schemaCache
                .AsCacheQueryable()
                .Where(c => c.Value.SchemaGroupId == groupId)
                .Select(c => new
                {
                    Id = c.Key,
                    c.Value.Version,
                    c.Value.Contents,
                    c.Value.CreatedBy,
                    c.Value.CreatedOn,
                })
                .ToList();

            return schemas.Select(s => new SchemaDto
            {
                Id = s.Id,
                Version = s.Version,
                Contents = s.Contents,
                CreatedBy = s.CreatedBy,
                CreatedOn = s.CreatedOn,
            });
        }

        public async Task<SchemaDto> GetLatestVersion(string groupName)
        {
            long groupId = await this.GetGroupId(groupName);
            ICache<long, SchemaCacheItem> schemaCache = this.Ignite.GetCache<long, SchemaCacheItem>(this.schemaCacheName);
            var schema = schemaCache
                .AsCacheQueryable()
                .Where(c => c.Value.SchemaGroupId == groupId)
                .OrderByDescending(c => c.Value.Version)
                .Select(c => new
                {
                    Id = c.Key,
                    c.Value.Version,
                    c.Value.Contents,
                    c.Value.CreatedBy,
                    c.Value.CreatedOn,
                })
                .FirstOrDefault();

            if (schema == null)
            {
                return null;
            }

            return new SchemaDto
            {
                Id = schema.Id,
                Version = schema.Version,
                Contents = schema.Contents,
                CreatedBy = schema.CreatedBy,
                CreatedOn = schema.CreatedOn,
            };
        }

        private Task<long> GetGroupId(string groupName)
        {
            ICache<long, SchemaGroupCacheItem> groupCache = this.Ignite.GetCache<long, SchemaGroupCacheItem>(this.groupCacheName);
            long groupId = groupCache
                .AsCacheQueryable()
                .Where(c => c.Value.Name.ToUpper() == groupName.ToUpper())
                .Select(c => c.Key)
                .First();

            return Task.FromResult(groupId);
        }
    }
}
