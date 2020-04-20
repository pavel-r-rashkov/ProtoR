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
    using ProtoR.Domain.ConfigurationSetAggregate;
    using ProtoR.Domain.GlobalConfigurationAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using ProtoR.Domain.SeedWork;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public class ConfigurationRepository : IConfigurationSetRepository
    {
        private readonly IIgnite ignite;
        private readonly string configurationCacheName;
        private readonly string ruleConfigurationGroupCacheName;

        public ConfigurationRepository(
            IIgnite ignite,
            IIgniteConfigurationProvider configurationProvider)
        {
            this.ignite = ignite;
            this.configurationCacheName = configurationProvider.ConfigurationCacheName;
            this.ruleConfigurationGroupCacheName = configurationProvider.RuleConfigurationCacheName;
        }

        public async Task<long> Add(ConfigurationSet configuration)
        {
            var configurationCacheItem = new ConfigurationCacheItem
            {
                SchemaGroupId = configuration.SchemaGroupId,
                ShouldInherit = configuration.ShouldInherit,
                ForwardCompatible = configuration.ForwardCompatible,
                BackwardCompatible = configuration.BackwardCompatible,
                Transitive = configuration.Transitive,
            };

            ICache<long, ConfigurationCacheItem> configurationCache = this.ignite.GetCache<long, ConfigurationCacheItem>(this.configurationCacheName);
            IAtomicSequence configurationCacheIdGenerator = this.ignite.GetAtomicSequence(
                $"{typeof(ConfigurationCacheItem).Name.ToUpperInvariant()}{CacheConstants.IdSequenceSufix}",
                0,
                true);
            long configurationId = configurationCacheIdGenerator.Increment();

            ICache<long, RuleConfigurationCacheItem> ruleConfigurationCache = this.ignite.GetCache<long, RuleConfigurationCacheItem>(this.ruleConfigurationGroupCacheName);
            IAtomicSequence ruleConfigurationCacheIdGenerator = this.ignite.GetAtomicSequence(
                $"{typeof(RuleConfigurationCacheItem).Name.ToUpperInvariant()}{CacheConstants.IdSequenceSufix}",
                0,
                true);

            IEnumerable<KeyValuePair<long, RuleConfigurationCacheItem>> ruleConfigurationCacheItems = configuration
                .GetRulesConfiguration()
                .Select(c => new KeyValuePair<long, RuleConfigurationCacheItem>(
                    ruleConfigurationCacheIdGenerator.Increment(),
                    new RuleConfigurationCacheItem
                    {
                        ConfigurationId = configurationId,
                        RuleCode = c.Key.ToString(),
                        ShouldInherit = c.Value.ShouldInherit,
                        Severity = c.Value.Severity.Id,
                    }));

            await configurationCache.PutAsync(configurationId, configurationCacheItem);
            await ruleConfigurationCache.PutAllAsync(ruleConfigurationCacheItems);

            return configurationId;
        }

        public Task<ConfigurationSet> GetById(long id)
        {
            ICache<long, ConfigurationCacheItem> configurationCache = this.ignite.GetCache<long, ConfigurationCacheItem>(this.configurationCacheName);
            ConfigurationCacheItem configurationCacheItem = configurationCache.Get(id);

            return Task.FromResult(this.HydrateConfiguration(id, configurationCacheItem));
        }

        public Task<ConfigurationSet> GetBySchemaGroupId(long? groupId)
        {
            ICache<long, ConfigurationCacheItem> configurationCache = this.ignite.GetCache<long, ConfigurationCacheItem>(this.configurationCacheName);
            ICacheEntry<long, ConfigurationCacheItem> configurationCacheItem = configurationCache
                .AsCacheQueryable()
                .Where(c => c.Value.SchemaGroupId == groupId)
                .FirstOrDefault();

            return Task.FromResult(this.HydrateConfiguration(configurationCacheItem.Key, configurationCacheItem.Value));
        }

        public async Task Update(ConfigurationSet configuration)
        {
            var configurationCacheItem = new ConfigurationCacheItem
            {
                SchemaGroupId = configuration.SchemaGroupId,
                ShouldInherit = configuration.ShouldInherit,
                ForwardCompatible = configuration.ForwardCompatible,
                BackwardCompatible = configuration.BackwardCompatible,
                Transitive = configuration.Transitive,
            };

            ICache<long, ConfigurationCacheItem> configurationCache = this.ignite.GetCache<long, ConfigurationCacheItem>(this.configurationCacheName);
            await configurationCache.PutAsync(configuration.Id, configurationCacheItem);

            ICache<long, RuleConfigurationCacheItem> ruleConfigurationCache = this.ignite.GetCache<long, RuleConfigurationCacheItem>(this.ruleConfigurationGroupCacheName);
            Dictionary<string, RuleConfig> updatedRules = configuration
                .GetRulesConfiguration()
                .ToDictionary(r => r.Key.ToString(), r => r.Value);

            IEnumerable<KeyValuePair<long, RuleConfigurationCacheItem>> ruleConfigurationCacheItems = ruleConfigurationCache
                .AsCacheQueryable()
                .Where(c => c.Value.ConfigurationId == configuration.Id)
                .ToList()
                .Select(c =>
                {
                    RuleConfig updatedRule = updatedRules[c.Value.RuleCode];
                    RuleConfigurationCacheItem cacheItem = c.Value;

                    cacheItem.ShouldInherit = updatedRule.ShouldInherit;
                    cacheItem.Severity = updatedRule.Severity.Id;

                    return new KeyValuePair<long, RuleConfigurationCacheItem>(c.Key, cacheItem);
                });

            await ruleConfigurationCache.PutAllAsync(ruleConfigurationCacheItems);
        }

        private ConfigurationSet HydrateConfiguration(long id, ConfigurationCacheItem configurationCacheItem)
        {
            var severities = Enumeration
                .GetAll<Severity>()
                .ToDictionary(s => s.Id);

            ICache<long, RuleConfigurationCacheItem> ruleConfigurationCache = this.ignite.GetCache<long, RuleConfigurationCacheItem>(this.ruleConfigurationGroupCacheName);
            Dictionary<RuleCode, RuleConfig> rulesConfiguration = ruleConfigurationCache
                .AsCacheQueryable()
                .Where(c => c.Value.ConfigurationId == id)
                .ToList()
                .ToDictionary(
                    c => (RuleCode)Enum.Parse(typeof(RuleCode), c.Value.RuleCode),
                    c => new RuleConfig(c.Value.ShouldInherit, severities[c.Value.Severity]));

            return new ConfigurationSet(
                id,
                rulesConfiguration,
                configurationCacheItem.SchemaGroupId,
                configurationCacheItem.ShouldInherit,
                configurationCacheItem.ForwardCompatible,
                configurationCacheItem.BackwardCompatible,
                configurationCacheItem.Transitive);
        }
    }
}
