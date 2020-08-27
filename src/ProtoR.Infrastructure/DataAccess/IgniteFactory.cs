namespace ProtoR.Infrastructure.DataAccess
{
    using System;
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
    using Apache.Ignite.Core.Ssl;
    using Autofac;
    using MediatR;
    using Microsoft.Extensions.Options;
    using ProtoR.Application.Registry;
    using ProtoR.Infrastructure.DataAccess.CacheItems;
    using ProtoR.Infrastructure.DataAccess.DependencyInjection;

    public class IgniteFactory : IIgniteFactory
    {
        private const string DefaultStoreType = "jks";
        private readonly IgniteExternalConfiguration externalConfiguration;
        private readonly IMediator mediator;
        private readonly Serilog.ILogger logger;
        private IIgnite ignite;

        public IgniteFactory(
            IOptions<IgniteExternalConfiguration> externalConfiguration,
            IMediator mediator,
            Serilog.ILogger logger)
        {
            this.externalConfiguration = externalConfiguration.Value;
            this.mediator = mediator;
            this.logger = logger.ForContext(
                Serilog.Core.Constants.SourceContextPropertyName,
                CacheConstants.IgniteLogSource);
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
                this.logger.Information("Cluster is disabled, activating");
                cluster.SetActive(true);
                this.logger.Information("Cluster activated");
            }
            else
            {
                this.logger.Information("Cluster is activated, changing topology");
                var baseLine = cluster.GetBaselineTopology();
                baseLine.Add(cluster.GetLocalNode());
                cluster.SetBaselineTopology(baseLine);
                this.logger.Information("Topology changed");
            }

            this.InitializeCaches();
            this.logger.Information("Caches initialized");
            this.CreateInitialData().GetAwaiter().GetResult();
            this.logger.Information("Initial data created");

            var services = cluster.ForNodes(cluster.GetNodes()).GetServices();
            services.DeployClusterSingleton(nameof(IClusterSingletonService), new ClusterSingletonService());
            this.logger.Information("Deployed service {0}", nameof(IClusterSingletonService));
        }

        private async Task CreateInitialData()
        {
            await this.mediator.Send(new InitRegistryCommand());
        }

        private void InitializeCaches()
        {
            this.ignite.GetOrCreateCache<long, ConfigurationCacheItem>(this.externalConfiguration.CacheNames.ConfigurationCacheName);
            this.ignite.GetOrCreateCache<long, RuleConfigurationCacheItem>(this.externalConfiguration.CacheNames.RuleConfigurationCacheName);
            this.ignite.GetOrCreateCache<long, SchemaCacheItem>(this.externalConfiguration.CacheNames.SchemaCacheName);
            this.ignite.GetOrCreateCache<long, SchemaGroupCacheItem>(this.externalConfiguration.CacheNames.SchemaGroupCacheName);
            this.ignite.GetOrCreateCache<long, UserCacheItem>(this.externalConfiguration.CacheNames.UserCacheName);
            this.ignite.GetOrCreateCache<UserRoleKey, EmptyCacheItem>(this.externalConfiguration.CacheNames.UserRoleCacheName);
            this.ignite.GetOrCreateCache<long, RoleCacheItem>(this.externalConfiguration.CacheNames.RoleCacheName);
            this.ignite.GetOrCreateCache<RolePermissionKey, EmptyCacheItem>(this.externalConfiguration.CacheNames.RolePermissionCacheName);
            this.ignite.GetOrCreateCache<long, ClientCacheItem>(this.externalConfiguration.CacheNames.ClientCacheName);
            this.ignite.GetOrCreateCache<ClientRoleKey, EmptyCacheItem>(this.externalConfiguration.CacheNames.ClientRoleCacheName);
            this.ignite.GetOrCreateCache<string, KeyCacheItem>(this.externalConfiguration.CacheNames.KeyCacheName);

            this.CreateSequence<ConfigurationCacheItem>();
            this.CreateSequence<RuleConfigurationCacheItem>();
            this.CreateSequence<SchemaCacheItem>();
            this.CreateSequence<SchemaGroupCacheItem>();
            this.CreateSequence<UserCacheItem>();
            this.CreateSequence<RoleCacheItem>();
            this.CreateSequence<ClientCacheItem>();
            this.CreateSequence<KeyCacheItem>();
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
            SslContextFactory sslContextFactory = null;

            if (externalConfiguration.TlsConfiguration != null
                && !string.IsNullOrEmpty(externalConfiguration.TlsConfiguration.KeyStoreLocation))
            {
                sslContextFactory = new SslContextFactory(
                    externalConfiguration.TlsConfiguration.KeyStoreLocation,
                    externalConfiguration.TlsConfiguration.KeyStorePassword,
                    externalConfiguration.TlsConfiguration.TrustStoreLocation,
                    externalConfiguration.TlsConfiguration.TrustStorePassword)
                {
                    KeyStoreType = externalConfiguration.TlsConfiguration.KeyStoreType ?? DefaultStoreType,
                    TrustStoreType = externalConfiguration.TlsConfiguration.TrustStoreType ?? DefaultStoreType,
                };
            }

            // Authentication can be enabled only when persistence is active
            var authenticationEnabled = externalConfiguration.EnablePersistence;

            return new IgniteConfiguration
            {
                SslContextFactory = sslContextFactory,
                AuthenticationEnabled = authenticationEnabled,
                Logger = new IgniteSerilogLogger(this.logger),
                WorkDirectory = storagePath,
                BinaryConfiguration = new BinaryConfiguration
                {
                    Serializer = new BinaryReflectiveSerializer { ForceTimestamp = true },
                },
                PluginConfigurations = new[] { new AutoFacPluginConfiguration() },
                FailureHandler = new NoOpFailureHandler(),
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
                        externalConfiguration.CacheNames.SchemaCacheName,
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
                        externalConfiguration.CacheNames.SchemaGroupCacheName,
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
                        externalConfiguration.CacheNames.ConfigurationCacheName,
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
                        externalConfiguration.CacheNames.RuleConfigurationCacheName,
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
                        externalConfiguration.CacheNames.UserCacheName,
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
                                new QueryField("IsActive", typeof(bool)) { NotNull = true },
                                new QueryField("CreatedBy", typeof(string)),
                                new QueryField("CreatedOn", typeof(DateTime)) { NotNull = true },
                            },
                        })
                    {
                        AtomicityMode = CacheAtomicityMode.Transactional,
                        WriteSynchronizationMode = CacheWriteSynchronizationMode.FullSync,
                    },
                    new CacheConfiguration(
                        externalConfiguration.CacheNames.UserRoleCacheName,
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
                        externalConfiguration.CacheNames.RoleCacheName,
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
                        externalConfiguration.CacheNames.RolePermissionCacheName,
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
                        externalConfiguration.CacheNames.ClientCacheName,
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
                                new QueryField("IsActive", typeof(bool)) { NotNull = false },
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
                        externalConfiguration.CacheNames.ClientRoleCacheName,
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
                    new CacheConfiguration(
                        externalConfiguration.CacheNames.KeyCacheName,
                        new QueryEntity
                        {
                            KeyType = typeof(long),
                            ValueType = typeof(KeyCacheItem),
                            Fields = new[]
                            {
                                new QueryField(nameof(KeyCacheItem.XmlElement), typeof(string)) { NotNull = true },
                                new QueryField(nameof(KeyCacheItem.FriendlyName), typeof(string)),
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
