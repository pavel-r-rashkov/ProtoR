namespace ProtoR.Web.Infrastructure
{
    using System;
    using ProtoR.Infrastructure.DataAccess;

    public class IgniteConfiguration : IIgniteConfiguration
    {
        private string schemaCacheName;
        private string schemaGroupCacheName;
        private string configurationCacheName;
        private string ruleConfigurationCacheName;
        private string userCacheName;
        private string roleCacheName;
        private string userRoleCacheName;
        private string rolePermissionCacheName;
        private string categoryCacheName;
        private string clientCacheName;
        private string clientRoleCacheName;
        private string clientCategoryCacheName;
        private string userCategoryCacheName;
        private int discoveryPort;
        private int communicationPort;
        private string nodeEndpoints;
        private string storagePath;

        public string SchemaCacheName
        {
            get
            {
                return this.schemaCacheName;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"{nameof(this.SchemaCacheName)} cannot be null or empty");
                }

                this.schemaCacheName = value;
            }
        }

        public string SchemaGroupCacheName
        {
            get
            {
                return this.schemaGroupCacheName;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"{nameof(this.SchemaGroupCacheName)} cannot be null or empty");
                }

                this.schemaGroupCacheName = value;
            }
        }

        public string ConfigurationCacheName
        {
            get
            {
                return this.configurationCacheName;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"{nameof(this.ConfigurationCacheName)} cannot be null or empty");
                }

                this.configurationCacheName = value;
            }
        }

        public string RuleConfigurationCacheName
        {
            get
            {
                return this.ruleConfigurationCacheName;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"{nameof(this.RuleConfigurationCacheName)} cannot be null or empty");
                }

                this.ruleConfigurationCacheName = value;
            }
        }

        public string UserCacheName
        {
            get
            {
                return this.userCacheName;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"{nameof(this.UserCacheName)} cannot be null or empty");
                }

                this.userCacheName = value;
            }
        }

        public string RoleCacheName
        {
            get
            {
                return this.roleCacheName;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"{nameof(this.RoleCacheName)} cannot be null or empty");
                }

                this.roleCacheName = value;
            }
        }

        public string UserRoleCacheName
        {
            get
            {
                return this.userRoleCacheName;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"{nameof(this.UserRoleCacheName)} cannot be null or empty");
                }

                this.userRoleCacheName = value;
            }
        }

        public int DiscoveryPort
        {
            get
            {
                return this.discoveryPort;
            }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException($"Invalid {nameof(this.DiscoveryPort)}");
                }

                this.discoveryPort = value;
            }
        }

        public int CommunicationPort
        {
            get
            {
                return this.communicationPort;
            }

            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException($"Invalid {nameof(this.CommunicationPort)}");
                }

                this.communicationPort = value;
            }
        }

        public string NodeEndpoints
        {
            get
            {
                return this.nodeEndpoints;
            }

            set
            {
                this.nodeEndpoints = value ?? throw new ArgumentNullException($"{nameof(this.NodeEndpoints)} cannot be null");
            }
        }

        public string StoragePath
        {
            get
            {
                return this.storagePath;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"{nameof(this.StoragePath)} cannot be null or empty");
                }

                this.storagePath = value;
            }
        }

        public string RolePermissionCacheName
        {
            get
            {
                return this.rolePermissionCacheName;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"{nameof(this.RolePermissionCacheName)} cannot be null or empty");
                }

                this.rolePermissionCacheName = value;
            }
        }

        public string CategoryCacheName
        {
            get
            {
                return this.categoryCacheName;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"{nameof(this.CategoryCacheName)} cannot be null or empty");
                }

                this.categoryCacheName = value;
            }
        }

        public string ClientCacheName
        {
            get
            {
                return this.clientCacheName;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"{nameof(this.ClientCacheName)} cannot be null or empty");
                }

                this.clientCacheName = value;
            }
        }

        public string ClientRoleCacheName
        {
            get
            {
                return this.clientRoleCacheName;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"{nameof(this.ClientRoleCacheName)} cannot be null or empty");
                }

                this.clientRoleCacheName = value;
            }
        }

        public string ClientCategoryCacheName
        {
            get
            {
                return this.clientCategoryCacheName;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"{nameof(this.ClientCategoryCacheName)} cannot be null or empty");
                }

                this.clientCategoryCacheName = value;
            }
        }

        public string UserCategoryCacheName
        {
            get
            {
                return this.userCategoryCacheName;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"{nameof(this.UserCategoryCacheName)} cannot be null or empty");
                }

                this.userCategoryCacheName = value;
            }
        }

        public bool EnablePersistence { get; set; }
    }
}
