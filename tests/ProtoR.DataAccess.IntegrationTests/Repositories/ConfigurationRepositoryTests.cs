namespace ProtoR.DataAccess.IntegrationTests.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Linq;
    using ProtoR.DataAccess.IntegrationTests.Fixtures;
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using ProtoR.Infrastructure.DataAccess.CacheItems;
    using ProtoR.Infrastructure.DataAccess.Repositories;
    using Xunit;

    [Collection(CollectionNames.IgniteCollection)]
    public sealed class ConfigurationRepositoryTests : IDisposable
    {
        private readonly IgniteFixture igniteFixture;
        private readonly ConfigurationRepository repository;
        private readonly ICache<long, ConfigurationCacheItem> configurationCache;
        private readonly ICache<long, RuleConfigurationCacheItem> ruleConfigurationCache;

        public ConfigurationRepositoryTests(IgniteFixture igniteFixture)
        {
            this.igniteFixture = igniteFixture;
            this.repository = new ConfigurationRepository(this.igniteFixture.IgniteFactory, this.igniteFixture.Configuration);
            this.configurationCache = this.igniteFixture.IgniteFactory.Instance().GetCache<long, ConfigurationCacheItem>(this.igniteFixture.Configuration.ConfigurationCacheName);
            this.ruleConfigurationCache = this.igniteFixture.IgniteFactory.Instance().GetCache<long, RuleConfigurationCacheItem>(this.igniteFixture.Configuration.RuleConfigurationCacheName);
        }

        public void Dispose()
        {
            this.igniteFixture.CleanIgniteCache();
        }

        [Fact]
        public async Task Add_ShouldInsertConfiguration()
        {
            var rulesConfigiguration = new Dictionary<RuleCode, RuleConfiguration>();
            var configuration = new Configuration(
                default,
                rulesConfigiguration,
                1,
                new GroupConfiguration(true, false, false, false));

            long id = await this.repository.Add(configuration);

            ConfigurationCacheItem cacheItem = this.configurationCache.Get(id);
            Assert.NotNull(cacheItem);
            Assert.Equal(1, cacheItem.SchemaGroupId);
            Assert.False(cacheItem.Inherit);
            Assert.True(cacheItem.ForwardCompatible);
            Assert.False(cacheItem.BackwardCompatible);
            Assert.False(cacheItem.Transitive);
        }

        [Fact]
        public async Task Add_ShouldInsertRuleConfiguration()
        {
            var rulesConfigiguration = new Dictionary<RuleCode, RuleConfiguration>()
            {
                { RuleCode.PB0001, new RuleConfiguration(false, Severity.Error) },
                { RuleCode.PB0002, new RuleConfiguration(false, Severity.Warning) },
            };
            var configuration = new Configuration(
                default,
                rulesConfigiguration,
                1,
                new GroupConfiguration(true, false, false, false));

            long id = await this.repository.Add(configuration);

            List<RuleConfigurationCacheItem> ruleConfigurationCacheItems = this.ruleConfigurationCache
                .AsCacheQueryable()
                .Where(c => c.Value.ConfigurationId == id)
                .ToList()
                .Select(c => c.Value)
                .ToList();

            Assert.Equal(2, ruleConfigurationCacheItems.Count);
            RuleConfigurationCacheItem ruleConfigCacheItem = ruleConfigurationCacheItems.First(r => r.RuleCode == RuleCode.PB0001.ToString());
            Assert.False(ruleConfigCacheItem.Inherit);
            Assert.Equal(Severity.Error.Id, ruleConfigCacheItem.Severity);
        }

        [Fact]
        public async Task GetById_ShouldReturnConfiguration()
        {
            var id = 100;
            var schemaGroupId = 10;
            var inherit = true;
            var forwardCompatible = true;
            var backwardCompatible = true;
            var transitive = true;
            await this.configurationCache.PutAsync(id, new ConfigurationCacheItem
            {
                SchemaGroupId = schemaGroupId,
                Inherit = inherit,
                ForwardCompatible = forwardCompatible,
                BackwardCompatible = backwardCompatible,
                Transitive = transitive,
            });

            Configuration configuration = await this.repository.GetById(id);

            Assert.NotNull(configuration);
            Assert.Equal(schemaGroupId, configuration.SchemaGroupId);
            Assert.Equal(inherit, configuration.GroupConfiguration.Inherit);
            Assert.Equal(forwardCompatible, configuration.GroupConfiguration.ForwardCompatible);
            Assert.Equal(backwardCompatible, configuration.GroupConfiguration.BackwardCompatible);
            Assert.Equal(transitive, configuration.GroupConfiguration.Transitive);
        }

        [Fact]
        public async Task GetById_ShouldReturnRulesConfiguration()
        {
            var id = 100;
            await this.configurationCache.PutAsync(id, new ConfigurationCacheItem
            {
                SchemaGroupId = 10,
                Inherit = true,
                ForwardCompatible = true,
                BackwardCompatible = true,
                Transitive = true,
            });
            await this.ruleConfigurationCache.PutAsync(200, new RuleConfigurationCacheItem
            {
                ConfigurationId = id,
                RuleCode = RuleCode.PB0001.ToString(),
                Inherit = true,
                Severity = Severity.Error.Id,
            });

            Configuration configuration = await this.repository.GetById(id);

            IReadOnlyDictionary<RuleCode, RuleConfiguration> rulesConfiguration = configuration.GetRulesConfiguration();
            Assert.Single(rulesConfiguration.Keys);
            RuleConfiguration ruleConfiguration = rulesConfiguration[RuleCode.PB0001];
            Assert.True(ruleConfiguration.Inherit);
            Assert.Equal(Severity.Error, ruleConfiguration.Severity);
        }

        [Fact]
        public async Task Update_ShouldUpdateConfiguration()
        {
            var id = 100;
            await this.configurationCache.PutAsync(id, new ConfigurationCacheItem
            {
                SchemaGroupId = 10,
                Inherit = true,
                ForwardCompatible = true,
                BackwardCompatible = false,
                Transitive = true,
            });
            Configuration configuration = await this.repository.GetById(id);

            configuration.SetGroupConfiguration(new GroupConfiguration(false, true, false, false));
            await this.repository.Update(configuration);

            ConfigurationCacheItem cacheItem = this.configurationCache.Get(id);

            Assert.True(cacheItem.BackwardCompatible);
            Assert.False(cacheItem.ForwardCompatible);
        }

        [Fact]
        public async Task Update_ShouldUpdateRuleConfigurations()
        {
            var id = 100;
            await this.configurationCache.PutAsync(id, new ConfigurationCacheItem
            {
                SchemaGroupId = 10,
                Inherit = true,
                ForwardCompatible = true,
                BackwardCompatible = false,
                Transitive = true,
            });
            await this.ruleConfigurationCache.PutAsync(200, new RuleConfigurationCacheItem
            {
                ConfigurationId = id,
                RuleCode = RuleCode.PB0001.ToString(),
                Inherit = true,
                Severity = Severity.Error.Id,
            });
            Configuration configuration = await this.repository.GetById(id);

            configuration.SetRulesConfiguration(new Dictionary<RuleCode, RuleConfiguration>
            {
                { RuleCode.PB0001, new RuleConfiguration(false, Severity.Warning) },
            });
            await this.repository.Update(configuration);

            IEnumerable<RuleConfigurationCacheItem> ruleCacheItems = this.ruleConfigurationCache
                .AsCacheQueryable()
                .Where(c => c.Value.ConfigurationId == id)
                .ToList()
                .Select(c => c.Value);

            Assert.Single(ruleCacheItems);
            var ruleCacheItem = ruleCacheItems.First();
            Assert.False(ruleCacheItem.Inherit);
            Assert.Equal(Severity.Warning.Id, ruleCacheItem.Severity);
        }

        [Fact]
        public async Task GetBySchemaGroupId_ShouldReturnConfiguration()
        {
            var schemaGroupId = 10;
            var configurationId = 100;
            await this.configurationCache.PutAsync(configurationId, new ConfigurationCacheItem
            {
                SchemaGroupId = schemaGroupId,
                Inherit = true,
                ForwardCompatible = true,
                BackwardCompatible = true,
                Transitive = true,
            });
            await this.ruleConfigurationCache.PutAsync(200, new RuleConfigurationCacheItem
            {
                ConfigurationId = configurationId,
                RuleCode = RuleCode.PB0001.ToString(),
                Inherit = true,
                Severity = Severity.Error.Id,
            });

            Configuration configuration = await this.repository.GetBySchemaGroupId(schemaGroupId);

            IReadOnlyDictionary<RuleCode, RuleConfiguration> rulesConfiguration = configuration.GetRulesConfiguration();
            Assert.Single(rulesConfiguration.Keys);
            RuleConfiguration ruleConfiguration = rulesConfiguration[RuleCode.PB0001];
            Assert.True(ruleConfiguration.Inherit);
            Assert.Equal(Severity.Error, ruleConfiguration.Severity);
        }
    }
}
