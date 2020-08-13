namespace ProtoR.Infrastructure.DataAccess
{
    public class IgniteExternalConfiguration
    {
        public string SchemaCacheName { get; set; }

        public string SchemaGroupCacheName { get; set; }

        public string ConfigurationCacheName { get; set; }

        public string RuleConfigurationCacheName { get; set; }

        public string RoleCacheName { get; set; }

        public string UserCacheName { get; set; }

        public string UserRoleCacheName { get; set; }

        public string UserCategoryCacheName { get; set; }

        public string RolePermissionCacheName { get; set; }

        public string CategoryCacheName { get; set; }

        public string ClientCacheName { get; set; }

        public string ClientRoleCacheName { get; set; }

        public string ClientCategoryCacheName { get; set; }

        public int DiscoveryPort { get; set; }

        public int CommunicationPort { get; set; }

        public string NodeEndpoints { get; set; }

        public string StoragePath { get; set; }

        public bool EnablePersistence { get; set; }
    }
}