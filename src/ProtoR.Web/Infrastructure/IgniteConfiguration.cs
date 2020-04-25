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
                if (value == null)
                {
                    throw new ArgumentNullException($"{nameof(this.NodeEndpoints)} cannot be null");
                }

                this.nodeEndpoints = value;
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
    }
}
