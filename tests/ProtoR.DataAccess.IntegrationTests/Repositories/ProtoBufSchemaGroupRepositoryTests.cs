namespace ProtoR.DataAccess.IntegrationTests.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Linq;
    using AutoFixture;
    using Google.Protobuf.Reflection;
    using Moq;
    using ProtoR.Application;
    using ProtoR.DataAccess.IntegrationTests.Fixtures;
    using ProtoR.Domain.CategoryAggregate;
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Infrastructure.DataAccess.CacheItems;
    using ProtoR.Infrastructure.DataAccess.Repositories;
    using Xunit;
    using Version = ProtoR.Domain.SchemaGroupAggregate.Schemas.Version;

    [Collection(CollectionNames.IgniteCollection)]
    public sealed class ProtoBufSchemaGroupRepositoryTests : IDisposable
    {
        private readonly IgniteFixture igniteFixture;
        private readonly ProtoBufSchemaGroupRepository repository;
        private readonly IUserProvider userProviderStub = new UserProviderStub();
        private readonly Fixture fixture = new Fixture();
        private readonly ICache<long, SchemaCacheItem> schemaCache;
        private readonly ICache<long, SchemaGroupCacheItem> schemaGroupCache;
        private readonly ICache<long, ConfigurationCacheItem> configurationCache;
        private readonly ICache<long, RuleConfigurationCacheItem> ruleConfigurationCache;

        public ProtoBufSchemaGroupRepositoryTests(IgniteFixture igniteFixture)
        {
            this.igniteFixture = igniteFixture;

            this.repository = new ProtoBufSchemaGroupRepository(
                this.igniteFixture.IgniteFactory,
                this.userProviderStub,
                this.igniteFixture.Configuration);

            this.fixture.Customizations.Add(new UtcRandomDateTimeSequenceGenerator());

            this.schemaCache = this.igniteFixture.IgniteFactory
                .Instance()
                .GetCache<long, SchemaCacheItem>(this.igniteFixture.Configuration.Value.SchemaCacheName);

            this.schemaGroupCache = this.igniteFixture.IgniteFactory
                .Instance()
                .GetCache<long, SchemaGroupCacheItem>(this.igniteFixture.Configuration.Value.SchemaGroupCacheName);

            this.configurationCache = this.igniteFixture.IgniteFactory
                .Instance()
                .GetCache<long, ConfigurationCacheItem>(this.igniteFixture.Configuration.Value.ConfigurationCacheName);

            this.ruleConfigurationCache = this.igniteFixture.IgniteFactory
                .Instance()
                .GetCache<long, RuleConfigurationCacheItem>(this.igniteFixture.Configuration.Value.RuleConfigurationCacheName);
        }

        public void Dispose()
        {
            this.igniteFixture.CleanIgniteCache();
        }

        [Fact]
        public async Task Add_ShouldCreateSchemaGroup()
        {
            var name = "Schema Group Name";

            long id = await this.repository.Add(new ProtoBufSchemaGroup(name, Category.DefaultCategoryId));

            Assert.True(id > 0);
            SchemaGroupCacheItem insertedItem = this.schemaGroupCache.Get(id);
            Assert.NotNull(insertedItem);
            Assert.Equal(name, insertedItem.Name);
            Assert.Equal(this.userProviderStub.GetCurrentUserName(), insertedItem.CreatedBy);
        }

        [Fact]
        public async Task GetByName_ShouldReturnSchemaGroup()
        {
            var groupId = 100;
            var name = "Schema Group Name";
            await this.InsertSchemaGroup(groupId, name);

            SchemaGroup<ProtoBufSchema, FileDescriptorSet> schemaGroup = await this.repository.GetByName(name);

            Assert.NotNull(schemaGroup);
            Assert.Equal(groupId, schemaGroup.Id);
            Assert.Equal(name, schemaGroup.Name);
        }

        [Fact]
        public async Task GetByName_WithMultipleSchemas_ShouldReturnAssociatedSchemas()
        {
            var groupId = 100;
            var name = "Schema Group Name";
            await this.InsertSchemaGroup(groupId, name);
            await this.InsertSchema(200, groupId, 1);
            await this.InsertSchema(201, groupId, 2);

            SchemaGroup<ProtoBufSchema, FileDescriptorSet> schemaGroup = await this.repository.GetByName(name);

            Assert.True(schemaGroup.Schemas.Count == 2);
        }

        [Fact]
        public async Task Update_WithNewSchemas_ShouldInsertNewSchemas()
        {
            var name = "Test Group Name";
            await this.repository.Add(new ProtoBufSchemaGroup(name, Category.DefaultCategoryId));
            ProtoBufSchemaGroup schemaGroup = await this.repository.GetByName(name);

            var schemaFactoryMock = new Mock<ISchemaFactory<ProtoBufSchema, FileDescriptorSet>>();
            schemaFactoryMock
                .Setup(f => f.CreateNew(It.IsAny<Version>(), It.IsAny<string>()))
                .Returns(new ProtoBufSchema(0, new Version(1), string.Empty));

            schemaGroup.AddSchema(
                "syntax = \"proto3\";",
                this.CreateGroupConfiguration(),
                this.CreateRuleConfiguration());

            await this.repository.Update(schemaGroup);

            var schemas = this.schemaCache
                .AsCacheQueryable()
                .Where(c => c.Value.SchemaGroupId == schemaGroup.Id)
                .ToList();

            Assert.NotEmpty(schemas);
        }

        [Fact]
        public async Task DeleteGroup_ShouldDeleteGroupAndAssociatedItems()
        {
            var group = this.fixture.Create<SchemaGroupCacheItem>();
            var groupId = 1;
            this.schemaGroupCache.Put(groupId, group);

            var schema = this.fixture.Create<SchemaCacheItem>();
            schema.SchemaGroupId = groupId;
            this.schemaCache.Put(1, schema);

            var configuration = this.fixture.Create<ConfigurationCacheItem>();
            configuration.SchemaGroupId = groupId;
            this.configurationCache.Put(1, configuration);

            var ruleConfiguration = this.fixture.Create<RuleConfigurationCacheItem>();
            ruleConfiguration.ConfigurationId = 1;
            this.ruleConfigurationCache.Put(1, ruleConfiguration);

            ProtoBufSchemaGroup groupAggregate = await this.repository.GetByName(group.Name);
            await this.repository.Delete(groupAggregate);

            Assert.Empty(this.schemaGroupCache.AsCacheQueryable().ToList());
            Assert.Empty(this.schemaCache.AsCacheQueryable().ToList());
            Assert.Empty(this.configurationCache.AsCacheQueryable().ToList());
            Assert.Empty(this.ruleConfigurationCache.AsCacheQueryable().ToList());
        }

        private async Task InsertSchemaGroup(long id, string name)
        {
            await this.schemaGroupCache.PutAsync(id, new SchemaGroupCacheItem
            {
                Name = name,
                CreatedBy = "Test user",
                CreatedOn = DateTime.UtcNow,
            });
        }

        private async Task InsertSchema(long id, long groupId, int version)
        {
            await this.schemaCache.PutAsync(id, new SchemaCacheItem
            {
                SchemaGroupId = groupId,
                Version = version,
                Contents = string.Empty,
                CreatedBy = "Test user",
                CreatedOn = DateTime.UtcNow,
            });
        }

        private GroupConfiguration CreateGroupConfiguration()
        {
            return new GroupConfiguration(true, false, false, false);
        }

        private Dictionary<RuleCode, RuleConfiguration> CreateRuleConfiguration()
        {
            return RuleFactory
                .GetProtoBufRules()
                .ToDictionary(r => r.Code, r => new RuleConfiguration(false, Severity.Info));
        }
    }
}
