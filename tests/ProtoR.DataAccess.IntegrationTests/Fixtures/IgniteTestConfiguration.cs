namespace ProtoR.DataAccess.IntegrationTests.Fixtures
{
    using ProtoR.Infrastructure.DataAccess;

    public class IgniteTestConfiguration : IIgniteConfiguration
    {
        public string SchemaCacheName => "PROTOR_SCHEMA_CACHE_INTEGRATION";

        public string SchemaGroupCacheName => "PROTOR_SCHEMA_GROUP_CACHE_INTEGRATION";

        public string ConfigurationCacheName => "PROTOR_CONFIGURATION_CACHE_INTEGRATION";

        public string RuleConfigurationCacheName => "PROTOR_RULE_CONFIGURATION_CACHE_INTEGRATION";

        public string UserCacheName => "PROTOR_USER_CACHE_INTEGRATION";

        public string RoleCacheName => "PROTOR_ROLE_CACHE_INTEGRATION";

        public string UserRoleCacheName => "PROTOR_USER_ROLE_CACHE_INTEGRATION";

        public string RolePermissionCacheName => "ROLE_PERMISSION_CACHE_INTEGRATION";

        public string CategoryCacheName => "CATEGORY_CACHE_INTEGRATION";

        public string ClientCacheName => "CLIENT_CACHE_INTEGRATION";

        public string ClientRoleCacheName => "CLIENT_ROLE_CACHE_INTEGRATION";

        public string ClientCategoryCacheName => "CLIENT_CATEGORY_CACHE_INTEGRATION";

        public string UserCategoryCacheName => "USER_CATEGORY_CACHE_INTEGRATION";

        public int DiscoveryPort => 10100;

        public int CommunicationPort => 9100;

        public string NodeEndpoints => "127.0.0.1:10100";

        public string StoragePath => @"/tmp/protor-cache-integration";

        public bool EnablePersistence => true;
    }
}
