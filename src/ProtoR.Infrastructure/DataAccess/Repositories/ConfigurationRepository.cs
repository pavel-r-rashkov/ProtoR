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
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using ProtoR.Domain.SeedWork;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly IIgnite ignite;
        private readonly string configurationCacheName;
        private readonly string ruleConfigurationGroupCacheName;

        public ConfigurationRepository(
            IIgniteFactory igniteFactory,
            IIgniteConfiguration configurationProvider)
        {
            this.ignite = igniteFactory.Instance();
            this.configurationCacheName = configurationProvider.ConfigurationCacheName;
            this.ruleConfigurationGroupCacheName = configurationProvider.RuleConfigurationCacheName;
        }

        public async Task<long> Add(Configuration configuration)
        {
            var configurationCacheItem = new ConfigurationCacheItem
            {
                SchemaGroupId = configuration.SchemaGroupId,
                Inherit = configuration.GroupConfiguration.Inherit,
                ForwardCompatible = configuration.GroupConfiguration.ForwardCompatible,
                BackwardCompatible = configuration.GroupConfiguration.BackwardCompatible,
                Transitive = configuration.GroupConfiguration.Transitive,
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
                        Inherit = c.Value.Inherit,
                        Severity = c.Value.Severity.Id,
                    }));

            await configurationCache.PutAsync(configurationId, configurationCacheItem);
            await ruleConfigurationCache.PutAllAsync(ruleConfigurationCacheItems);

            return configurationId;
        }

        public async Task<Configuration> GetById(long id)
        {
            var configurationCache = this.ignite.GetCache<long, ConfigurationCacheItem>(this.configurationCacheName);
            var result = await configurationCache.TryGetAsync(id);

            if (!result.Success)
            {
                return null;
            }

            return this.HydrateConfiguration(id, result.Value);
        }

        public Task<Configuration> GetBySchemaGroupId(long? groupId)
        {
            ICache<long, ConfigurationCacheItem> configurationCache = this.ignite.GetCache<long, ConfigurationCacheItem>(this.configurationCacheName);
            ICacheEntry<long, ConfigurationCacheItem> configurationCacheItem = configurationCache
                .AsCacheQueryable()
                .Where(c => c.Value.SchemaGroupId == groupId)
                .FirstOrDefault();

            if (configurationCacheItem == null)
            {
                return Task.FromResult((Configuration)null);
            }

            return Task.FromResult(this.HydrateConfiguration(configurationCacheItem.Key, configurationCacheItem.Value));
        }

        public async Task Update(Configuration configuration)
        {
            var configurationCacheItem = new ConfigurationCacheItem
            {
                SchemaGroupId = configuration.SchemaGroupId,
                Inherit = configuration.GroupConfiguration.Inherit,
                ForwardCompatible = configuration.GroupConfiguration.ForwardCompatible,
                BackwardCompatible = configuration.GroupConfiguration.BackwardCompatible,
                Transitive = configuration.GroupConfiguration.Transitive,
            };

            ICache<long, ConfigurationCacheItem> configurationCache = this.ignite.GetCache<long, ConfigurationCacheItem>(this.configurationCacheName);
            await configurationCache.PutAsync(configuration.Id, configurationCacheItem);

            ICache<long, RuleConfigurationCacheItem> ruleConfigurationCache = this.ignite.GetCache<long, RuleConfigurationCacheItem>(this.ruleConfigurationGroupCacheName);
            Dictionary<string, RuleConfiguration> updatedRules = configuration
                .GetRulesConfiguration()
                .ToDictionary(r => r.Key.ToString(), r => r.Value);

            IEnumerable<KeyValuePair<long, RuleConfigurationCacheItem>> ruleConfigurationCacheItems = ruleConfigurationCache
                .AsCacheQueryable()
                .Where(c => c.Value.ConfigurationId == configuration.Id)
                .ToList()
                .Select(c =>
                {
                    RuleConfiguration updatedRule = updatedRules[c.Value.RuleCode];
                    RuleConfigurationCacheItem cacheItem = c.Value;

                    cacheItem.Inherit = updatedRule.Inherit;
                    cacheItem.Severity = updatedRule.Severity.Id;

                    return new KeyValuePair<long, RuleConfigurationCacheItem>(c.Key, cacheItem);
                });

            await ruleConfigurationCache.PutAllAsync(ruleConfigurationCacheItems);
        }

        private Configuration HydrateConfiguration(long id, ConfigurationCacheItem configurationCacheItem)
        {
            var severities = Enumeration
                .GetAll<Severity>()
                .ToDictionary(s => s.Id);

            ICache<long, RuleConfigurationCacheItem> ruleConfigurationCache = this.ignite.GetCache<long, RuleConfigurationCacheItem>(this.ruleConfigurationGroupCacheName);
            Dictionary<RuleCode, RuleConfiguration> rulesConfiguration = ruleConfigurationCache
                .AsCacheQueryable()
                .Where(c => c.Value.ConfigurationId == id)
                .ToList()
                .ToDictionary(
                    c => (RuleCode)Enum.Parse(typeof(RuleCode), c.Value.RuleCode),
                    c => new RuleConfiguration(c.Value.Inherit, severities[c.Value.Severity]));

            return new Configuration(
                id,
                rulesConfiguration,
                configurationCacheItem.SchemaGroupId,
                new GroupConfiguration(
                    configurationCacheItem.ForwardCompatible,
                    configurationCacheItem.BackwardCompatible,
                    configurationCacheItem.Transitive,
                    configurationCacheItem.Inherit));
        }
    }
}
