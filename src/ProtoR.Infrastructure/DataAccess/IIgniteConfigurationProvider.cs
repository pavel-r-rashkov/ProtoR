namespace ProtoR.Infrastructure.DataAccess
{
    using System.Collections.Generic;

    public interface IIgniteConfigurationProvider
    {
        string SchemaCacheName { get; }

        string SchemaGroupCacheName { get; }

        string ConfigurationCacheName { get; }

        string RuleConfigurationCacheName { get; }

        int DiscoveryPort { get; }

        int CommunicationPort { get; }

        IEnumerable<string> NodeEndpoints { get; }

        string StoragePath { get; }
    }
}
