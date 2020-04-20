namespace ProtoR.DataAccess.IntegrationTests.Fixtures
{
    using System;
    using Apache.Ignite.Core;
    using Apache.Ignite.Core.Cache;
    using ProtoR.Infrastructure.DataAccess;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public sealed class IgniteFixture : IDisposable
    {
        public IgniteFixture()
        {
            this.Configuration = new IgniteTestConfiguration();
            var igniteFactory = new IgniteFactory(this.Configuration);
            this.Ignite = igniteFactory.InitalizeIgnite();
        }

        public IIgnite Ignite { get; }

        public IIgniteConfigurationProvider Configuration { get; }

        public void CleanIgniteCache()
        {
            this.CleanCache<long, SchemaGroupCacheItem>(this.Configuration.SchemaGroupCacheName);
            this.CleanCache<long, SchemaCacheItem>(this.Configuration.SchemaCacheName);
            this.CleanCache<long, ConfigurationCacheItem>(this.Configuration.ConfigurationCacheName);
            this.CleanCache<long, RuleConfigurationCacheItem>(this.Configuration.RuleConfigurationCacheName);
        }

        public void Dispose()
        {
            this.Ignite.Dispose();
        }

        private void CleanCache<TKey, TValue>(string cacheName)
        {
            var cache = this.Ignite.GetCache<TKey, TValue>(cacheName);
            cache.RemoveAll();
        }
    }
}
