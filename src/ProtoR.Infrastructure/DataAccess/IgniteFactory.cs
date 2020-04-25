namespace ProtoR.Infrastructure.DataAccess
{
    using System;
    using System.Diagnostics;
    using System.IO;
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
    using MediatR;
    using ProtoR.Application.Configuration;
    using ProtoR.Infrastructure.DataAccess.CacheItems;
    using ProtoR.Infrastructure.DataAccess.DependencyInjection;

    public class IgniteFactory : IIgniteFactory
    {
        private readonly IIgniteConfiguration externalConfiguration;
        private readonly IMediator mediator;
        private IIgnite ignite;

        public IgniteFactory(
            IIgniteConfiguration externalConfiguration,
            IMediator mediator)
        {
            this.externalConfiguration = externalConfiguration;
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

        public void InitalizeIgnite()
        {
            IgniteConfiguration configuration = this.CreateIgniteConfig(this.externalConfiguration);
            this.ignite = Ignition.Start(configuration);
            ICluster cluster = this.ignite.GetCluster();

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

            this.CreateGlobalConfiguration().GetAwaiter().GetResult();

            var services = cluster.ForNodes(cluster.GetNodes()).GetServices();
            services.DeployClusterSingleton(nameof(IClusterSingletonService), new ClusterSingletonService());
        }

        private async Task CreateGlobalConfiguration()
        {
            await this.mediator.Send(new CreateGlobalConfigurationCommand());
        }

        private IgniteConfiguration CreateIgniteConfig(IIgniteConfiguration externalConfiguration)
        {
            string storagePath = externalConfiguration.StoragePath;

            return new IgniteConfiguration
            {
                BinaryConfiguration = new BinaryConfiguration
                {
                    Serializer = new BinaryReflectiveSerializer { ForceTimestamp = true },
                },
                PluginConfigurations = new[] { new AutoFacPluginConfiguration() },
                FailureHandler = new NoOpFailureHandler(), // For Debug
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
                    StoragePath = Path.Combine(storagePath, "storage"),
                    WalPath = Path.Combine(storagePath, "wal"),
                    WalArchivePath = Path.Combine(storagePath, "wal-archive"),
                    DefaultDataRegionConfiguration = new DataRegionConfiguration
                    {
                        Name = "defaultRegion",
                        PersistenceEnabled = true,
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
                                new QueryField("CreatedBy", typeof(string)) { NotNull = true },
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
                },
            };
        }
    }
}
