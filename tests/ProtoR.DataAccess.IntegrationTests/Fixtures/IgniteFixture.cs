namespace ProtoR.DataAccess.IntegrationTests.Fixtures
{
    using System;
    using System.IO;
    using System.Reflection;
    using MediatR;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using Moq;
    using ProtoR.Infrastructure.DataAccess;
    using ProtoR.Infrastructure.DataAccess.CacheItems;
    using Serilog;

    public sealed class IgniteFixture : IDisposable
    {
        public IgniteFixture()
        {
            var directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var settingsLocation = Path.Combine(directory, "integration-appsettings.json");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(settingsLocation)
                .Build();

            var externalConfiguration = new IgniteExternalConfiguration();
            configuration.GetSection("Ignite").Bind(externalConfiguration);
            this.Configuration = Options.Create(externalConfiguration);
            var logger = new LoggerConfiguration().CreateLogger();

            this.IgniteFactory = new IgniteFactory(
                this.Configuration,
                new Mock<IMediator>().Object,
                logger);
            this.IgniteFactory.InitalizeIgnite();
        }

        public IIgniteFactory IgniteFactory { get; }

        public IOptions<IgniteExternalConfiguration> Configuration { get; }

        public void CleanIgniteCache()
        {
            this.CleanCache<long, SchemaGroupCacheItem>(this.Configuration.Value.CacheNames.SchemaGroupCacheName);
            this.CleanCache<long, SchemaCacheItem>(this.Configuration.Value.CacheNames.SchemaCacheName);
            this.CleanCache<long, ConfigurationCacheItem>(this.Configuration.Value.CacheNames.ConfigurationCacheName);
            this.CleanCache<long, RuleConfigurationCacheItem>(this.Configuration.Value.CacheNames.RuleConfigurationCacheName);
            this.CleanCache<long, UserCacheItem>(this.Configuration.Value.CacheNames.UserCacheName);
            this.CleanCache<UserRoleKey, EmptyCacheItem>(this.Configuration.Value.CacheNames.UserRoleCacheName);
            this.CleanCache<long, RoleCacheItem>(this.Configuration.Value.CacheNames.RoleCacheName);
            this.CleanCache<RolePermissionKey, EmptyCacheItem>(this.Configuration.Value.CacheNames.RolePermissionCacheName);
            this.CleanCache<long, ClientCacheItem>(this.Configuration.Value.CacheNames.ClientCacheName);
            this.CleanCache<ClientRoleKey, EmptyCacheItem>(this.Configuration.Value.CacheNames.ClientRoleCacheName);
            this.CleanCache<long, KeyCacheItem>(this.Configuration.Value.CacheNames.KeyCacheName);
            this.CleanCache<string, GrantCacheItem>(this.Configuration.Value.CacheNames.GrantStoreCacheName);
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
