namespace ProtoR.DataAccess.IntegrationTests.DataProviders
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core.Cache;
    using AutoFixture;
    using ProtoR.Application.Configuration;
    using ProtoR.DataAccess.IntegrationTests.Fixtures;
    using ProtoR.Infrastructure.DataAccess.CacheItems;
    using ProtoR.Infrastructure.DataAccess.DataProviders;
    using Xunit;

    [Collection(CollectionNames.IgniteCollection)]
    public sealed class ConfigurationDataProviderTests : IDisposable
    {
        private readonly IgniteFixture igniteFixture;
        private readonly ConfigurationDataProvider dataProvider;
        private readonly ICache<long, ConfigurationCacheItem> configurationCache;
        private readonly ICache<long, RuleConfigurationCacheItem> rulesConfigurationCache;
        private readonly ICache<long, SchemaGroupCacheItem> groupCache;
        private readonly Fixture fixture = new Fixture();

        public ConfigurationDataProviderTests(IgniteFixture igniteFixture)
        {
            this.igniteFixture = igniteFixture;
            this.dataProvider = new ConfigurationDataProvider(this.igniteFixture.IgniteFactory, this.igniteFixture.Configuration);
            this.configurationCache = this.igniteFixture.IgniteFactory.Instance().GetCache<long, ConfigurationCacheItem>(this.igniteFixture.Configuration.ConfigurationCacheName);
            this.rulesConfigurationCache = this.igniteFixture.IgniteFactory.Instance().GetCache<long, RuleConfigurationCacheItem>(this.igniteFixture.Configuration.RuleConfigurationCacheName);
            this.groupCache = this.igniteFixture.IgniteFactory.Instance().GetCache<long, SchemaGroupCacheItem>(this.igniteFixture.Configuration.SchemaGroupCacheName);
            this.fixture.Customizations.Add(new UtcRandomDateTimeSequenceGenerator());
        }

        public void Dispose()
        {
            this.igniteFixture.CleanIgniteCache();
        }

        [Fact]
        public async Task GetById_ShouldReturnConfiguration()
        {
            var configurationId = 10;
            this.configurationCache.Put(configurationId, this.fixture.Create<ConfigurationCacheItem>());
            var rulesConfigurationCount = 5;

            for (int i = 1; i <= rulesConfigurationCount; i++)
            {
                var ruleConfiguration = this.fixture.Create<RuleConfigurationCacheItem>();
                ruleConfiguration.ConfigurationId = configurationId;
                this.rulesConfigurationCache.Put(i, ruleConfiguration);
            }

            ConfigurationDto configuration = await this.dataProvider.GetById(configurationId);

            Assert.NotNull(configuration);
            Assert.Equal(rulesConfigurationCount, configuration.RuleConfigurations.Count());
        }

        [Fact]
        public async Task GetConfigByGroupName_ShouldReturnGroupConfiguration()
        {
            var groupId = 1;
            var group = this.fixture.Create<SchemaGroupCacheItem>();
            this.groupCache.Put(groupId, group);

            var configurationId = 10;
            var configuration = this.fixture.Create<ConfigurationCacheItem>();
            configuration.SchemaGroupId = groupId;
            this.configurationCache.Put(configurationId, configuration);
            var rulesConfigurationCount = 5;

            for (int i = 1; i <= rulesConfigurationCount; i++)
            {
                var ruleConfiguration = this.fixture.Create<RuleConfigurationCacheItem>();
                ruleConfiguration.ConfigurationId = configurationId;
                this.rulesConfigurationCache.Put(i, ruleConfiguration);
            }

            ConfigurationDto result = await this.dataProvider.GetConfigByGroupName(group.Name);

            Assert.NotNull(configuration);
            Assert.Equal(rulesConfigurationCount, result.RuleConfigurations.Count());
        }

        [Fact]
        public async Task GetGlobalConfig_ShouldReturnGlobalConfiguration()
        {
            var configuration = this.fixture.Create<ConfigurationCacheItem>();
            configuration.SchemaGroupId = null;
            this.configurationCache.Put(10, configuration);

            ConfigurationDto result = await this.dataProvider.GetGlobalConfig();

            Assert.NotNull(result);
        }
    }
}
