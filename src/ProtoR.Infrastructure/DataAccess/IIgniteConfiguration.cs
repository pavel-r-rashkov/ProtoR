namespace ProtoR.Infrastructure.DataAccess
{
    public interface IIgniteConfiguration
    {
        string SchemaCacheName { get; }

        string SchemaGroupCacheName { get; }

        string ConfigurationCacheName { get; }

        string RuleConfigurationCacheName { get; }

        string RoleCacheName { get; }

        string UserCacheName { get; }

        string UserRoleCacheName { get; }

        string UserCategoryCacheName { get; }

        string RolePermissionCacheName { get; }

        string CategoryCacheName { get; }

        string ClientCacheName { get; }

        string ClientRoleCacheName { get; }

        string ClientCategoryCacheName { get; }

        int DiscoveryPort { get; }

        int CommunicationPort { get; }

        string NodeEndpoints { get; }

        string StoragePath { get; }

        bool EnablePersistence { get; }
    }
}
