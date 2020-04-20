namespace ProtoR.Infrastructure.DataAccess.DataProviders
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Linq;
    using ProtoR.Application.Configuration;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public class ConfigurationDataProvider : IConfigurationDataProvider
    {
        private readonly IIgnite ignite;
        private readonly string groupCacheName;
        private readonly string configurationCacheName;
        private readonly string ruleConfigurationCacheName;

        public ConfigurationDataProvider(
            IIgnite ignite,
            IIgniteConfigurationProvider configurationProvider)
        {
            this.ignite = ignite;
            this.groupCacheName = configurationProvider.SchemaGroupCacheName;
            this.configurationCacheName = configurationProvider.ConfigurationCacheName;
            this.ruleConfigurationCacheName = configurationProvider.RuleConfigurationCacheName;
        }

        public async Task<ConfigurationDto> GetById(long id)
        {
            return await this.GetByCondition(c => c.Key == id);
        }

        public async Task<ConfigurationDto> GetConfigByGroupName(string groupName)
        {
            ICache<long, SchemaGroupCacheItem> groupCache = this.ignite.GetCache<long, SchemaGroupCacheItem>(this.groupCacheName);

            var g = groupCache.AsCacheQueryable().ToList();

            long groupId = groupCache
                .AsCacheQueryable()
                .Where(c => c.Value.Name.ToUpper() == groupName.ToUpper())
                .Select(c => c.Key)
                .First();

            return await this.GetByCondition(c => c.Value.SchemaGroupId == groupId);
        }

        public async Task<ConfigurationDto> GetGlobalConfig()
        {
            return await this.GetByCondition(c => c.Value.SchemaGroupId == null);
        }

        private Task<ConfigurationDto> GetByCondition(Func<ICacheEntry<long, ConfigurationCacheItem>, bool> condition)
        {
            ICache<long, ConfigurationCacheItem> configurationCache = this.ignite.GetCache<long, ConfigurationCacheItem>(this.configurationCacheName);
            var configurationProjection = configurationCache
                .AsCacheQueryable()
                .Where(condition)
                .Select(c => new
                {
                    Id = c.Key,
                    c.Value.SchemaGroupId,
                    c.Value.Transitive,
                    c.Value.ForwardCompatible,
                    c.Value.BackwardCompatible,
                })
                .FirstOrDefault();

            if (configurationProjection == null)
            {
                return null;
            }

            var configuration = new ConfigurationDto
            {
                Id = configurationProjection.Id,
                GroupId = configurationProjection.SchemaGroupId,
                Transitive = configurationProjection.Transitive,
                ForwardCompatible = configurationProjection.ForwardCompatible,
                BackwardCompatible = configurationProjection.BackwardCompatible,
            };

            ICache<long, RuleConfigurationCacheItem> ruleConfigurationCache = this.ignite.GetCache<long, RuleConfigurationCacheItem>(this.ruleConfigurationCacheName);
            configuration.RuleConfigurations = ruleConfigurationCache
                .AsCacheQueryable()
                .Where(c => c.Value.ConfigurationId == configuration.Id)
                .Select(c => new
                {
                    c.Value.RuleCode,
                    c.Value.Severity,
                    c.Value.ShouldInherit,
                })
                .ToList()
                .Select(c => new RuleConfigurationDto
                {
                    RuleCode = c.RuleCode,
                    Severity = c.Severity,
                    Inherit = c.ShouldInherit,
                });

            return Task.FromResult(configuration);
        }
    }
}
