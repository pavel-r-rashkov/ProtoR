namespace ProtoR.Infrastructure.DataAccess
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Apache.Ignite.Core;
    using Apache.Ignite.Core.Binary;
    using Apache.Ignite.Core.Cache.Configuration;
    using Apache.Ignite.Core.Cluster;
    using Apache.Ignite.Core.Communication.Tcp;
    using Apache.Ignite.Core.Configuration;
    using Apache.Ignite.Core.Discovery.Tcp;
    using Apache.Ignite.Core.Discovery.Tcp.Static;
    using Apache.Ignite.Core.Failure;
    using Autofac;
    using MediatR;
    using Microsoft.Extensions.Options;
    using ProtoR.Application.Registry;
    using ProtoR.Infrastructure.DataAccess.CacheItems;
    using ProtoR.Infrastructure.DataAccess.DependencyInjection;

    public class IgniteFactory : IIgniteFactory
    {
        private readonly IgniteExternalConfiguration externalConfiguration;
        private readonly IMediator mediator;
        private IIgnite ignite;

        public IgniteFactory(
            IOptions<IgniteExternalConfiguration> externalConfiguration,
            IMediator mediator)
        {
            this.externalConfiguration = externalConfiguration.Value;
            this.mediator = mediator;
        }

        public IIgnite Instance()
        {
            if (this.ignite == null)
            {
                throw new InvalidOperationException("Ignite is not initialized");
            }

            return this.ignite;
        }

        public void SetAutoFacPlugin(ILifetimeScope scope)
        {
            var plugin = this.Instance().GetPlugin<AutoFacPlugin>(nameof(AutoFacPluginProvider));
            plugin.Scope = scope;
        }

        public void InitalizeIgnite()
        {
            IgniteConfiguration configuration = this.CreateIgniteConfig(this.externalConfiguration);
            this.ignite = Ignition.Start(configuration);
            ICluster cluster = this.ignite.GetCluster();
            cluster.SetBaselineAutoAdjustEnabledFlag(false);

            if (!cluster.IsActive())
            {
                Debug.WriteLine("-------- Activate cluster");
                cluster.SetActive(true);
            }
            else
            {
                Debug.WriteLine("-------- Join / Change topology");
                var baseLine = cluster.GetBaselineTopology();
                baseLine.Add(cluster.GetLocalNode());
                cluster.SetBaselineTopology(baseLine);
            }

            this.InitializeCaches();
            this.CreateInitialData().GetAwaiter().GetResult();

            var services = cluster.ForNodes(cluster.GetNodes()).GetServices();
            services.DeployClusterSingleton(nameof(IClusterSingletonService), new ClusterSingletonService());
        }

        private async Task CreateInitialData()
        {
            await this.mediator.Send(new InitRegistryCommand());
        }

        private void InitializeCaches()
        {
            this.ignite.GetOrCreateCache<long, ConfigurationCacheItem>(this.externalConfiguration.ConfigurationCacheName);
            this.ignite.GetOrCreateCache<long, RuleConfigurationCacheItem>(this.externalConfiguration.RuleConfigurationCacheName);
            this.ignite.GetOrCreateCache<long, SchemaCacheItem>(this.externalConfiguration.SchemaCacheName);
            this.ignite.GetOrCreateCache<long, SchemaGroupCacheItem>(this.externalConfiguration.SchemaGroupCacheName);
            this.ignite.GetOrCreateCache<long, UserCacheItem>(this.externalConfiguration.UserCacheName);
            this.ignite.GetOrCreateCache<UserRoleKey, EmptyCacheItem>(this.externalConfiguration.UserRoleCacheName);
            this.ignite.GetOrCreateCache<long, RoleCacheItem>(this.externalConfiguration.RoleCacheName);
            this.ignite.GetOrCreateCache<RolePermissionKey, EmptyCacheItem>(this.externalConfiguration.RolePermissionCacheName);
            this.ignite.GetOrCreateCache<long, ClientCacheItem>(this.externalConfiguration.ClientCacheName);
            this.ignite.GetOrCreateCache<ClientRoleKey, EmptyCacheItem>(this.externalConfiguration.ClientRoleCacheName);

            this.CreateSequence<ConfigurationCacheItem>();
            this.CreateSequence<RuleConfigurationCacheItem>();
            this.CreateSequence<SchemaCacheItem>();
            this.CreateSequence<SchemaGroupCacheItem>();
            this.CreateSequence<UserCacheItem>();
            this.CreateSequence<RoleCacheItem>();
            this.CreateSequence<ClientCacheItem>();
        }

        private void CreateSequence<T>(long initialValue = 0)
        {
            this.ignite.GetAtomicSequence(
                $"{typeof(T).Name.ToUpperInvariant()}{CacheConstants.IdSequenceSufix}",
                initialValue,
                true);
        }

        private IgniteConfiguration CreateIgniteConfig(IgniteExternalConfiguration externalConfiguration)
        {
            string storagePath = externalConfiguration.StoragePath;

            return new IgniteConfiguration
            {
                // Authentication can be enabled only when persistence is active
                AuthenticationEnabled = externalConfiguration.EnablePersistence,
                WorkDirectory = storagePath,
                BinaryConfiguration = new BinaryConfiguration
                {
                    Serializer = new BinaryReflectiveSerializer { ForceTimestamp = true },
                },
                PluginConfigurations = new[] { new AutoFacPluginConfiguration() },
                FailureHandler = new NoOpFailureHandler(), // TODO For Debug
                ClientMode = false,
                DiscoverySpi = new TcpDiscoverySpi
                {
                    LocalPort = externalConfiguration.DiscoveryPort,
                    IpFinder = new TcpDiscoveryStaticIpFinder
                    {
                        Endpoints = externalConfiguration.NodeEndpoints
                            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                            .ToList(),
                    },
                },
                CommunicationSpi = new TcpCommunicationSpi
                {
                    LocalPort = externalConfiguration.CommunicationPort,
                },
                DataStorageConfiguration = new DataStorageConfiguration
                {
                    DefaultDataRegionConfiguration = new DataRegionConfiguration
                    {
                        Name = "defaultRegion",
                        PersistenceEnabled = externalConfiguration.EnablePersistence,
                    },
                    DataRegionConfigurations = new[]
                    {
                        new DataRegionConfiguration
                        {
                            Name = "inMemoryRegion",
                        },
                    },
                },
                CacheConfiguration = new[]
                {
                    new CacheConfiguration(
                        externalConfiguration.SchemaCacheName,
                        new QueryEntity
                        {
                            KeyType = typeof(long),
                            KeyFieldName = "ID",
                            ValueType = typeof(SchemaCacheItem),
                            Fields = new[]
                            {
                                new QueryField("ID", typeof(long)) { IsKeyField = true },
                                new QueryField("SchemaGroupId", typeof(long)),
                                new QueryField("Version", typeof(int)),
                                new QueryField("Contents", typeof(string)),
                                new QueryField("CreatedBy", typeof(string)),
                                new QueryField("CreatedOn", typeof(DateTime)),
                            },
                        })
                    {
                        AtomicityMode = CacheAtomicityMode.Transactional,
                        WriteSynchronizationMode = CacheWriteSynchronizationMode.FullSync,
                    },
                    new CacheConfiguration(
                        externalConfiguration.SchemaGroupCacheName,
                        new QueryEntity
                        {
                            KeyType = typeof(long),
                            KeyFieldName = "ID",
                            ValueType = typeof(SchemaGroupCacheItem),
                            Fields = new[]
                            {
                                new QueryField("ID", typeof(long)) { IsKeyField = true },
                                new QueryField("Name", typeof(string)) { NotNull = true },
                                new QueryField("CreatedBy", typeof(string)),
                                new QueryField("CreatedOn", typeof(DateTime)) { NotNull = true },
                            },
                        })
                    {
                        AtomicityMode = CacheAtomicityMode.Transactional,
                        WriteSynchronizationMode = CacheWriteSynchronizationMode.FullSync,
                    },
                    new CacheConfiguration(
                        externalConfiguration.ConfigurationCacheName,
                        new QueryEntity
                        {
                            KeyType = typeof(long),
                            KeyFieldName = "ID",
                            ValueType = typeof(ConfigurationCacheItem),
                            Fields = new[]
                            {
                                new QueryField("ID", typeof(long)) { IsKeyField = true },
                                new QueryField("SchemaGroupId", typeof(long?)),
                                new QueryField("Inherit", typeof(bool)) { NotNull = true },
                                new QueryField("ForwardCompatible", typeof(bool)) { NotNull = true },
                                new QueryField("BackwardCompatible", typeof(bool)) { NotNull = true },
                                new QueryField("Transitive", typeof(bool)) { NotNull = true },
                            },
                        })
                    {
                        AtomicityMode = CacheAtomicityMode.Transactional,
                        WriteSynchronizationMode = CacheWriteSynchronizationMode.FullSync,
                    },
                    new CacheConfiguration(
                        externalConfiguration.RuleConfigurationCacheName,
                        new QueryEntity
                        {
                            KeyType = typeof(long),
                            KeyFieldName = "ID",
                            ValueType = typeof(RuleConfigurationCacheItem),
                            Fields = new[]
                            {
                                new QueryField("ID", typeof(long)) { IsKeyField = true },
                                new QueryField("RuleCode", typeof(string)) { NotNull = true },
                                new QueryField("ConfigurationId", typeof(long)) { NotNull = true },
                                new QueryField("Inherit", typeof(bool)) { NotNull = true },
                                new QueryField("Severity", typeof(int)) { NotNull = true },
                            },
                        })
                    {
                        AtomicityMode = CacheAtomicityMode.Transactional,
                        WriteSynchronizationMode = CacheWriteSynchronizationMode.FullSync,
                    },
                    new CacheConfiguration(
                        externalConfiguration.UserCacheName,
                        new QueryEntity
                        {
                            KeyType = typeof(long),
                            KeyFieldName = "ID",
                            ValueType = typeof(UserCacheItem),
                            Fields = new[]
                            {
                                new QueryField("ID", typeof(long)) { IsKeyField = true },
                                new QueryField("UserName", typeof(string)) { NotNull = true },
                                new QueryField("NormalizedUserName", typeof(string)) { NotNull = true },
                                new QueryField("PasswordHash", typeof(string)) { NotNull = true },
                                new QueryField("GroupRestrictions", typeof(string)) { NotNull = true },
                                new QueryField("CreatedBy", typeof(string)),
                                new QueryField("CreatedOn", typeof(DateTime)) { NotNull = true },
                            },
                        })
                    {
                        AtomicityMode = CacheAtomicityMode.Transactional,
                        WriteSynchronizationMode = CacheWriteSynchronizationMode.FullSync,
                    },
                    new CacheConfiguration(
                        externalConfiguration.UserRoleCacheName,
                        new QueryEntity
                        {
                            KeyType = typeof(UserRoleKey),
                            ValueType = typeof(EmptyCacheItem),
                            Fields = new[]
                            {
                                new QueryField("UserId", typeof(long)) { IsKeyField = true },
                                new QueryField("RoleId", typeof(long)) { IsKeyField = true },
                            },
                        })
                    {
                        AtomicityMode = CacheAtomicityMode.Transactional,
                        WriteSynchronizationMode = CacheWriteSynchronizationMode.FullSync,
                    },
                    new CacheConfiguration(
                        externalConfiguration.RoleCacheName,
                        new QueryEntity
                        {
                            KeyType = typeof(long),
                            KeyFieldName = "ID",
                            ValueType = typeof(RoleCacheItem),
                            Fields = new[]
                            {
                                new QueryField("ID", typeof(long)) { IsKeyField = true },
                                new QueryField("Name", typeof(string)) { NotNull = true },
                                new QueryField("NormalizedName", typeof(string)) { NotNull = true },
                                new QueryField("CreatedBy", typeof(string)),
                                new QueryField("CreatedOn", typeof(DateTime)) { NotNull = true },
                            },
                        })
                    {
                        AtomicityMode = CacheAtomicityMode.Transactional,
                        WriteSynchronizationMode = CacheWriteSynchronizationMode.FullSync,
                    },
                    new CacheConfiguration(
                        externalConfiguration.RolePermissionCacheName,
                        new QueryEntity
                        {
                            KeyType = typeof(RolePermissionKey),
                            ValueType = typeof(EmptyCacheItem),
                            Fields = new[]
                            {
                                new QueryField("RoleId", typeof(long)) { IsKeyField = true },
                                new QueryField("PermissionId", typeof(int)) { IsKeyField = true },
                            },
                        })
                    {
                        AtomicityMode = CacheAtomicityMode.Transactional,
                        WriteSynchronizationMode = CacheWriteSynchronizationMode.FullSync,
                    },
                    new CacheConfiguration(
                        externalConfiguration.ClientCacheName,
                        new QueryEntity
                        {
                            KeyType = typeof(long),
                            KeyFieldName = "ID",
                            ValueType = typeof(ClientCacheItem),
                            Fields = new[]
                            {
                                new QueryField("ID", typeof(long)) { IsKeyField = true },
                                new QueryField("ClientId", typeof(string)) { NotNull = true },
                                new QueryField("ClientName", typeof(string)) { NotNull = true },
                                new QueryField("Secret", typeof(string)) { NotNull = false },
                                new QueryField("GrantTypes", typeof(string)) { NotNull = true },
                                new QueryField("RedirectUris", typeof(string)) { NotNull = true },
                                new QueryField("PostLogoutRedirectUris", typeof(string)) { NotNull = true },
                                new QueryField("AllowedCorsOrigins", typeof(string)) { NotNull = true },
                                new QueryField("GroupRestrictions", typeof(string)) { NotNull = true },
                                new QueryField("CreatedBy", typeof(string)),
                                new QueryField("CreatedOn", typeof(DateTime)) { NotNull = true },
                            },
                        })
                    {
                        AtomicityMode = CacheAtomicityMode.Transactional,
                        WriteSynchronizationMode = CacheWriteSynchronizationMode.FullSync,
                    },
                    new CacheConfiguration(
                        externalConfiguration.ClientRoleCacheName,
                        new QueryEntity
                        {
                            KeyType = typeof(ClientRoleKey),
                            ValueType = typeof(EmptyCacheItem),
                            Fields = new[]
                            {
                                new QueryField("ClientId", typeof(long)) { IsKeyField = true },
                                new QueryField("RoleId", typeof(long)) { IsKeyField = true },
                            },
                        })
                    {
                        AtomicityMode = CacheAtomicityMode.Transactional,
                        WriteSynchronizationMode = CacheWriteSynchronizationMode.FullSync,
                    },
                },
            };
        }
    }
}
