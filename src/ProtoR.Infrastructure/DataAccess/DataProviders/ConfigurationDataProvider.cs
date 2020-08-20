namespace ProtoR.Infrastructure.DataAccess.DataProviders
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using Apache.Ignite.Linq;
    using Microsoft.Extensions.Options;
    using ProtoR.Application.Configuration;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public class ConfigurationDataProvider : BaseDataProvider, IConfigurationDataProvider
    {
        private readonly string groupCacheName;
        private readonly string configurationCacheName;
        private readonly string ruleConfigurationCacheName;

        public ConfigurationDataProvider(
            IIgniteFactory igniteFactory,
            IOptions<IgniteExternalConfiguration> configurationProvider)
            : base(igniteFactory, configurationProvider)
        {
            this.groupCacheName = this.ConfigurationProvider.CacheNames.SchemaGroupCacheName;
            this.configurationCacheName = this.ConfigurationProvider.CacheNames.ConfigurationCacheName;
            this.ruleConfigurationCacheName = this.ConfigurationProvider.CacheNames.RuleConfigurationCacheName;
        }

        public async Task<ConfigurationDto> GetById(long id)
        {
            return await this.GetByCondition(c => c.Key == id);
        }

        public async Task<ConfigurationDto> GetConfigByGroupName(string groupName)
        {
            ICache<long, SchemaGroupCacheItem> groupCache = this.Ignite.GetCache<long, SchemaGroupCacheItem>(this.groupCacheName);

            var g = groupCache.AsCacheQueryable().ToList();

            long groupId = groupCache
                .AsCacheQueryable()
                .Where(c => c.Value.Name.ToUpper() == groupName.ToUpper())
                .Select(c => c.Key)
                .FirstOrDefault();

            if (groupId == default)
            {
                return null;
            }

            return await this.GetByCondition(c => c.Value.SchemaGroupId == groupId);
        }

        public async Task<ConfigurationDto> GetGlobalConfig()
        {
            return await this.GetByCondition(c => c.Value.SchemaGroupId == null);
        }

        private Task<ConfigurationDto> GetByCondition(Func<ICacheEntry<long, ConfigurationCacheItem>, bool> condition)
        {
            ICache<long, ConfigurationCacheItem> configurationCache = this.Ignite.GetCache<long, ConfigurationCacheItem>(this.configurationCacheName);
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
                    c.Value.Inherit,
                })
                .FirstOrDefault();

            if (configurationProjection == null)
            {
                return Task.FromResult((ConfigurationDto)null);
            }

            var configuration = new ConfigurationDto
            {
                Id = configurationProjection.Id,
                GroupId = configurationProjection.SchemaGroupId,
                Transitive = configurationProjection.Transitive,
                ForwardCompatible = configurationProjection.ForwardCompatible,
                BackwardCompatible = configurationProjection.BackwardCompatible,
                Inherit = configurationProjection.Inherit,
            };

            ICache<long, RuleConfigurationCacheItem> ruleConfigurationCache = this.Ignite.GetCache<long, RuleConfigurationCacheItem>(this.ruleConfigurationCacheName);
            configuration.RuleConfigurations = ruleConfigurationCache
                .AsCacheQueryable()
                .Where(c => c.Value.ConfigurationId == configuration.Id)
                .Select(c => new
                {
                    c.Value.RuleCode,
                    c.Value.Severity,
                    c.Value.Inherit,
                })
                .ToList()
                .Select(c => new RuleConfigurationDto
                {
                    RuleCode = c.RuleCode,
                    Severity = c.Severity,
                    Inherit = c.Inherit,
                });

            return Task.FromResult(configuration);
        }
    }
}
