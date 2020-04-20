namespace ProtoR.Infrastructure.DataAccess
{
    using System;
    using System.IO;
    using System.Linq;
    using Apache.Ignite.Core;
    using Apache.Ignite.Core.Binary;
    using Apache.Ignite.Core.Cache.Configuration;
    using Apache.Ignite.Core.Cluster;
    using Apache.Ignite.Core.Communication.Tcp;
    using Apache.Ignite.Core.Configuration;
    using Apache.Ignite.Core.Discovery.Tcp;
    using Apache.Ignite.Core.Discovery.Tcp.Static;
    using Apache.Ignite.Core.Failure;
    using ProtoR.Infrastructure.DataAccess.CacheItems;

    public class IgniteFactory
    {
        private readonly IIgniteConfigurationProvider configurationProvider;

        public IgniteFactory(IIgniteConfigurationProvider configurationProvider)
        {
            this.configurationProvider = configurationProvider;
        }

        public IIgnite InitalizeIgnite()
        {
            IgniteConfiguration configuration = this.CreateIgniteConfig();
            IIgnite ignite = Ignition.Start(configuration);
            ICluster cluster = ignite.GetCluster();

            if (!cluster.IsActive())
            {
                cluster.SetActive(true);
            }
            else
            {
                // Console.WriteLine("-------- Join / Change topology");
                // var list = cluster.GetBaselineTopology();
                // list.Add(cluster.GetLocalNode());
                // cluster.SetBaselineTopology(list);
            }

            return ignite;
        }

        private IgniteConfiguration CreateIgniteConfig()
        {
            string storagePath = this.configurationProvider.StoragePath;

            return new IgniteConfiguration
            {
                BinaryConfiguration = new BinaryConfiguration
                {
                    Serializer = new BinaryReflectiveSerializer { ForceTimestamp = true },
                },
                FailureHandler = new NoOpFailureHandler(), // For Debug
                ClientMode = false,
                DiscoverySpi = new TcpDiscoverySpi
                {
                    LocalPort = this.configurationProvider.DiscoveryPort,
                    IpFinder = new TcpDiscoveryStaticIpFinder
                    {
                        Endpoints = this.configurationProvider.NodeEndpoints.ToList(),
                    },
                },
                CommunicationSpi = new TcpCommunicationSpi
                {
                    LocalPort = this.configurationProvider.CommunicationPort,
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
                        this.configurationProvider.SchemaCacheName,
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
                        this.configurationProvider.SchemaGroupCacheName,
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
                        this.configurationProvider.ConfigurationCacheName,
                        new QueryEntity
                        {
                            KeyType = typeof(long),
                            KeyFieldName = "ID",
                            ValueType = typeof(ConfigurationCacheItem),
                            Fields = new[]
                            {
                                new QueryField("ID", typeof(long)) { IsKeyField = true },
                                new QueryField("SchemaGroupId", typeof(long?)),
                                new QueryField("ShouldInherit", typeof(bool)) { NotNull = true },
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
                        this.configurationProvider.RuleConfigurationCacheName,
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
                                new QueryField("ShouldInherit", typeof(bool)) { NotNull = true },
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
