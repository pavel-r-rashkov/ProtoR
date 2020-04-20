namespace ProtoR.DataAccess.IntegrationTests.Fixtures
{
    using System.Collections.Generic;
    using ProtoR.Infrastructure.DataAccess;

    public class IgniteTestConfiguration : IIgniteConfigurationProvider
    {
        public string SchemaCacheName => "PROTOR_SCHEMA_CACHE_INTEGRATION";

        public string SchemaGroupCacheName => "PROTOR_SCHEMA_GROUP_CACHE_INTEGRATION";

        public string ConfigurationCacheName => "PROTOR_CONFIGURATION_CACHE_INTEGRATION";

        public string RuleConfigurationCacheName => "PROTOR_RULE_CONFIGURATION_CACHE_INTEGRATION";

        public int DiscoveryPort => 10100;

        public int CommunicationPort => 9100;

        public IEnumerable<string> NodeEndpoints => new[] { "127.0.0.1:10100" };

        public string StoragePath => @"/tmp/protor-cache-integration";
    }
}
