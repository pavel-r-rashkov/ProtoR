namespace ProtoR.DataAccess.IntegrationTests.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Linq;
    using ProtoR.DataAccess.IntegrationTests.Fixtures;
    using ProtoR.Domain.GlobalConfigurationAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
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
            this.repository = new ConfigurationRepository(this.igniteFixture.Ignite, this.igniteFixture.Configuration);
            this.configurationCache = this.igniteFixture.Ignite.GetCache<long, ConfigurationCacheItem>(this.igniteFixture.Configuration.ConfigurationCacheName);
            this.ruleConfigurationCache = this.igniteFixture.Ignite.GetCache<long, RuleConfigurationCacheItem>(this.igniteFixture.Configuration.RuleConfigurationCacheName);
        }

        public void Dispose()
        {
            this.igniteFixture.CleanIgniteCache();
        }

        [Fact]
        public async Task Add_ShouldInsertConfiguration()
        {
            var rulesConfigiguration = new Dictionary<RuleCode, RuleConfig>();
            var configuration = new ConfigurationSet(
                default,
                rulesConfigiguration,
                1,
                false,
                true,
                false,
                false);

            long id = await this.repository.Add(configuration);

            ConfigurationCacheItem cacheItem = this.configurationCache.Get(id);
            Assert.NotNull(cacheItem);
            Assert.Equal(1, cacheItem.SchemaGroupId);
            Assert.False(cacheItem.ShouldInherit);
            Assert.True(cacheItem.ForwardCompatible);
            Assert.False(cacheItem.BackwardCompatible);
            Assert.False(cacheItem.Transitive);
        }

        [Fact]
        public async Task Add_ShouldInsertRuleConfiguration()
        {
            var rulesConfigiguration = new Dictionary<RuleCode, RuleConfig>()
            {
                { RuleCode.PB0001, new RuleConfig(false, Severity.Error) },
                { RuleCode.PB0002, new RuleConfig(false, Severity.Warning) },
            };
            var configuration = new ConfigurationSet(
                default,
                rulesConfigiguration,
                1,
                false,
                true,
                false,
                false);

            long id = await this.repository.Add(configuration);

            List<RuleConfigurationCacheItem> ruleConfigurationCacheItems = this.ruleConfigurationCache
                .AsCacheQueryable()
                .Where(c => c.Value.ConfigurationId == id)
                .ToList()
                .Select(c => c.Value)
                .ToList();

            Assert.Equal(2, ruleConfigurationCacheItems.Count);
            RuleConfigurationCacheItem ruleConfigCacheItem = ruleConfigurationCacheItems.First(r => r.RuleCode == RuleCode.PB0001.ToString());
            Assert.False(ruleConfigCacheItem.ShouldInherit);
            Assert.Equal(Severity.Error.Id, ruleConfigCacheItem.Severity);
        }

        [Fact]
        public async Task GetById_ShouldReturnConfiguration()
        {
            var id = 100;
            var schemaGroupId = 10;
            var shouldInherit = true;
            var forwardCompatible = true;
            var backwardCompatible = true;
            var transitive = true;
            await this.configurationCache.PutAsync(id, new ConfigurationCacheItem
            {
                SchemaGroupId = schemaGroupId,
                ShouldInherit = shouldInherit,
                ForwardCompatible = forwardCompatible,
                BackwardCompatible = backwardCompatible,
                Transitive = transitive,
            });

            ConfigurationSet configuration = await this.repository.GetById(id);

            Assert.NotNull(configuration);
            Assert.Equal(schemaGroupId, configuration.SchemaGroupId);
            Assert.Equal(shouldInherit, configuration.ShouldInherit);
            Assert.Equal(forwardCompatible, configuration.ForwardCompatible);
            Assert.Equal(backwardCompatible, configuration.BackwardCompatible);
            Assert.Equal(transitive, configuration.Transitive);
        }

        [Fact]
        public async Task GetById_ShouldReturnRulesConfiguration()
        {
            var id = 100;
            await this.configurationCache.PutAsync(id, new ConfigurationCacheItem
            {
                SchemaGroupId = 10,
                ShouldInherit = true,
                ForwardCompatible = true,
                BackwardCompatible = true,
                Transitive = true,
            });
            await this.ruleConfigurationCache.PutAsync(200, new RuleConfigurationCacheItem
            {
                ConfigurationId = id,
                RuleCode = RuleCode.PB0001.ToString(),
                ShouldInherit = true,
                Severity = Severity.Error.Id,
            });

            ConfigurationSet configuration = await this.repository.GetById(id);

            IReadOnlyDictionary<RuleCode, RuleConfig> rulesConfiguration = configuration.GetRulesConfiguration();
            Assert.Single(rulesConfiguration.Keys);
            RuleConfig ruleConfiguration = rulesConfiguration[RuleCode.PB0001];
            Assert.True(ruleConfiguration.ShouldInherit);
            Assert.Equal(Severity.Error, ruleConfiguration.Severity);
        }

        [Fact]
        public async Task Update_ShouldUpdateConfiguration()
        {
            var id = 100;
            await this.configurationCache.PutAsync(id, new ConfigurationCacheItem
            {
                SchemaGroupId = 10,
                ShouldInherit = true,
                ForwardCompatible = true,
                BackwardCompatible = false,
                Transitive = true,
            });
            ConfigurationSet configuration = await this.repository.GetById(id);

            configuration.SetCompatibility(true, false);
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
                ShouldInherit = true,
                ForwardCompatible = true,
                BackwardCompatible = false,
                Transitive = true,
            });
            await this.ruleConfigurationCache.PutAsync(200, new RuleConfigurationCacheItem
            {
                ConfigurationId = id,
                RuleCode = RuleCode.PB0001.ToString(),
                ShouldInherit = true,
                Severity = Severity.Error.Id,
            });
            ConfigurationSet configuration = await this.repository.GetById(id);

            configuration.SetRulesConfiguration(new Dictionary<RuleCode, RuleConfig>
            {
                { RuleCode.PB0001, new RuleConfig(false, Severity.Warning) },
            });
            await this.repository.Update(configuration);

            IEnumerable<RuleConfigurationCacheItem> ruleCacheItems = this.ruleConfigurationCache
                .AsCacheQueryable()
                .Where(c => c.Value.ConfigurationId == id)
                .ToList()
                .Select(c => c.Value);

            Assert.Single(ruleCacheItems);
            var ruleCacheItem = ruleCacheItems.First();
            Assert.False(ruleCacheItem.ShouldInherit);
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
                ShouldInherit = true,
                ForwardCompatible = true,
                BackwardCompatible = true,
                Transitive = true,
            });
            await this.ruleConfigurationCache.PutAsync(200, new RuleConfigurationCacheItem
            {
                ConfigurationId = configurationId,
                RuleCode = RuleCode.PB0001.ToString(),
                ShouldInherit = true,
                Severity = Severity.Error.Id,
            });

            ConfigurationSet configuration = await this.repository.GetBySchemaGroupId(schemaGroupId);

            IReadOnlyDictionary<RuleCode, RuleConfig> rulesConfiguration = configuration.GetRulesConfiguration();
            Assert.Single(rulesConfiguration.Keys);
            RuleConfig ruleConfiguration = rulesConfiguration[RuleCode.PB0001];
            Assert.True(ruleConfiguration.ShouldInherit);
            Assert.Equal(Severity.Error, ruleConfiguration.Severity);
        }
    }
}
