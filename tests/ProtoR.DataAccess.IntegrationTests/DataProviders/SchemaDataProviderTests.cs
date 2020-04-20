namespace ProtoR.DataAccess.IntegrationTests.DataProviders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using AutoFixture;
    using ProtoR.Application.Schema;
    using ProtoR.DataAccess.IntegrationTests.Fixtures;
    using ProtoR.Infrastructure.DataAccess.CacheItems;
    using ProtoR.Infrastructure.DataAccess.DataProviders;
    using Xunit;

    [Collection(CollectionNames.IgniteCollection)]
    public sealed class SchemaDataProviderTests : IDisposable
    {
        private readonly IgniteFixture igniteFixture;
        private readonly SchemaDataProvider dataProvider;
        private readonly ICache<long, SchemaCacheItem> schemaCache;
        private readonly ICache<long, SchemaGroupCacheItem> groupCache;
        private readonly Fixture fixture = new Fixture();

        public SchemaDataProviderTests(IgniteFixture igniteFixture)
        {
            this.igniteFixture = igniteFixture;
            this.dataProvider = new SchemaDataProvider(this.igniteFixture.Ignite, this.igniteFixture.Configuration);
            this.schemaCache = this.igniteFixture.Ignite.GetCache<long, SchemaCacheItem>(this.igniteFixture.Configuration.SchemaCacheName);
            this.groupCache = this.igniteFixture.Ignite.GetCache<long, SchemaGroupCacheItem>(this.igniteFixture.Configuration.SchemaGroupCacheName);
            this.fixture.Customizations.Add(new UtcRandomDateTimeSequenceGenerator());
        }

        public void Dispose()
        {
            this.igniteFixture.CleanIgniteCache();
        }

        [Fact]
        public async Task GetByVersion_ShouldReturnSchemaWithVersion()
        {
            var group = this.fixture.Create<SchemaGroupCacheItem>();
            var groupId = 1;
            this.groupCache.Put(groupId, group);
            var schemas = this.fixture.CreateMany<SchemaCacheItem>().ToList();

            for (int i = 0; i < schemas.Count; i++)
            {
                schemas[i].SchemaGroupId = groupId;
                schemas[i].Version = i + 1;
                this.schemaCache.Put(i + 1, schemas[i]);
            }

            SchemaDto schema = await this.dataProvider.GetByVersion(group.Name, schemas[0].Version);

            Assert.NotNull(schema);
            Assert.Equal(schemas[0].Version, schema.Version);
        }

        [Fact]
        public async Task GetGroupSchemas_ShouldReturnListOfSchemasForAGroup()
        {
            var group = this.fixture.Create<SchemaGroupCacheItem>();
            var groupId = 1;
            this.groupCache.Put(groupId, group);
            var schemas = this.fixture.CreateMany<SchemaCacheItem>().ToList();

            for (int i = 0; i < schemas.Count; i++)
            {
                schemas[i].SchemaGroupId = groupId;
                this.schemaCache.Put(i + 1, schemas[i]);
            }

            IEnumerable<SchemaDto> result = await this.dataProvider.GetGroupSchemas(group.Name);

            Assert.NotNull(result);
            Assert.Equal(schemas.Count, result.Count());
        }

        [Fact]
        public async Task GetLatestVersion_ShouldReturnTheMostRecentSchemaForAGroup()
        {
            var group = this.fixture.Create<SchemaGroupCacheItem>();
            var groupId = 1;
            this.groupCache.Put(groupId, group);
            var schemas = this.fixture.CreateMany<SchemaCacheItem>().ToList();

            for (int i = 0; i < schemas.Count; i++)
            {
                schemas[i].SchemaGroupId = groupId;
                schemas[i].Version = i + 1;
                this.schemaCache.Put(i + 1, schemas[i]);
            }

            var lastSchema = schemas.Max(s => s.Version);

            SchemaDto result = await this.dataProvider.GetLatestVersion(group.Name);

            Assert.NotNull(result);
            Assert.Equal(lastSchema, result.Version);
        }
    }
}
