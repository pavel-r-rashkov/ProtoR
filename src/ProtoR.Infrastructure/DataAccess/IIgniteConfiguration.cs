namespace ProtoR.Infrastructure.DataAccess
{
    public interface IIgniteConfiguration
    {
        string SchemaCacheName { get; }

        string SchemaGroupCacheName { get; }

        string ConfigurationCacheName { get; }

        string RuleConfigurationCacheName { get; }

        int DiscoveryPort { get; }

        int CommunicationPort { get; }

        string NodeEndpoints { get; }

        string StoragePath { get; }
    }
}
