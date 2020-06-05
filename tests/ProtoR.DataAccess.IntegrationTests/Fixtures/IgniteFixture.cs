namespace ProtoR.DataAccess.IntegrationTests.Fixtures
{
    using System;
    using MediatR;
    using Moq;
    using ProtoR.Infrastructure.DataAccess;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public sealed class IgniteFixture : IDisposable
    {
        public IgniteFixture()
        {
            this.Configuration = new IgniteTestConfiguration();
            this.IgniteFactory = new IgniteFactory(this.Configuration, new Mock<IMediator>().Object);
            this.IgniteFactory.InitalizeIgnite();
        }

        public IIgniteFactory IgniteFactory { get; }

        public IIgniteConfiguration Configuration { get; }

        public void CleanIgniteCache()
        {
            this.CleanCache<long, SchemaGroupCacheItem>(this.Configuration.SchemaGroupCacheName);
            this.CleanCache<long, SchemaCacheItem>(this.Configuration.SchemaCacheName);
            this.CleanCache<long, ConfigurationCacheItem>(this.Configuration.ConfigurationCacheName);
            this.CleanCache<long, RuleConfigurationCacheItem>(this.Configuration.RuleConfigurationCacheName);
            this.CleanCache<long, UserCacheItem>(this.Configuration.UserCacheName);
            this.CleanCache<UserRoleKey, EmptyCacheItem>(this.Configuration.UserRoleCacheName);
            this.CleanCache<UserCategoryKey, EmptyCacheItem>(this.Configuration.UserCategoryCacheName);
            this.CleanCache<long, RoleCacheItem>(this.Configuration.RoleCacheName);
            this.CleanCache<RolePermissionKey, EmptyCacheItem>(this.Configuration.RolePermissionCacheName);
            this.CleanCache<long, CategoryCacheItem>(this.Configuration.CategoryCacheName);
            this.CleanCache<long, ClientCacheItem>(this.Configuration.ClientCacheName);
            this.CleanCache<ClientRoleKey, EmptyCacheItem>(this.Configuration.ClientRoleCacheName);
            this.CleanCache<ClientCategoryKey, EmptyCacheItem>(this.Configuration.ClientCategoryCacheName);
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
