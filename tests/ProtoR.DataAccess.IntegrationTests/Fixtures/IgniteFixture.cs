namespace ProtoR.DataAccess.IntegrationTests.Fixtures
{
    using System;
    using MediatR;
    using Microsoft.Extensions.Options;
    using Moq;
    using ProtoR.Infrastructure.DataAccess;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public sealed class IgniteFixture : IDisposable
    {
        public IgniteFixture()
        {
            var externalConfiguration = new IgniteExternalConfiguration
            {
                SchemaCacheName = "PROTOR_SCHEMA_CACHE_INTEGRATION",
                SchemaGroupCacheName = "PROTOR_SCHEMA_GROUP_CACHE_INTEGRATION",
                ConfigurationCacheName = "PROTOR_CONFIGURATION_CACHE_INTEGRATION",
                RuleConfigurationCacheName = "PROTOR_RULE_CONFIGURATION_CACHE_INTEGRATION",
                UserCacheName = "PROTOR_USER_CACHE_INTEGRATION",
                RoleCacheName = "PROTOR_ROLE_CACHE_INTEGRATION",
                UserRoleCacheName = "PROTOR_USER_ROLE_CACHE_INTEGRATION",
                RolePermissionCacheName = "ROLE_PERMISSION_CACHE_INTEGRATION",
                CategoryCacheName = "CATEGORY_CACHE_INTEGRATION",
                ClientCacheName = "CLIENT_CACHE_INTEGRATION",
                ClientRoleCacheName = "CLIENT_ROLE_CACHE_INTEGRATION",
                ClientCategoryCacheName = "CLIENT_CATEGORY_CACHE_INTEGRATION",
                UserCategoryCacheName = "USER_CATEGORY_CACHE_INTEGRATION",
                DiscoveryPort = 10100,
                CommunicationPort = 9100,
                NodeEndpoints = "127.0.0.1:10100",
                StoragePath = @"/tmp/protor-cache-integration",
                EnablePersistence = true,
            };
            this.Configuration = Options.Create(externalConfiguration);
            this.IgniteFactory = new IgniteFactory(this.Configuration, new Mock<IMediator>().Object);
            this.IgniteFactory.InitalizeIgnite();
        }

        public IIgniteFactory IgniteFactory { get; }

        public IOptions<IgniteExternalConfiguration> Configuration { get; }

        public void CleanIgniteCache()
        {
            this.CleanCache<long, SchemaGroupCacheItem>(this.Configuration.Value.SchemaGroupCacheName);
            this.CleanCache<long, SchemaCacheItem>(this.Configuration.Value.SchemaCacheName);
            this.CleanCache<long, ConfigurationCacheItem>(this.Configuration.Value.ConfigurationCacheName);
            this.CleanCache<long, RuleConfigurationCacheItem>(this.Configuration.Value.RuleConfigurationCacheName);
            this.CleanCache<long, UserCacheItem>(this.Configuration.Value.UserCacheName);
            this.CleanCache<UserRoleKey, EmptyCacheItem>(this.Configuration.Value.UserRoleCacheName);
            this.CleanCache<long, RoleCacheItem>(this.Configuration.Value.RoleCacheName);
            this.CleanCache<RolePermissionKey, EmptyCacheItem>(this.Configuration.Value.RolePermissionCacheName);
            this.CleanCache<long, ClientCacheItem>(this.Configuration.Value.ClientCacheName);
            this.CleanCache<ClientRoleKey, EmptyCacheItem>(this.Configuration.Value.ClientRoleCacheName);
        }

        public void Dispose()
        {
            this.IgniteFactory.Instance().Dispose();
        }

        private void CleanCache<TKey, TValue>(string cacheName)
        {
            var cache = this.IgniteFactory.Instance().GetCache<TKey, TValue>(cacheName);
            cache.RemoveAll();
        }
    }
}
