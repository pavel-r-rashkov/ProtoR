namespace ProtoR.Infrastructure.DataAccess
{
    public class IgniteExternalConfiguration
    {
        public int DiscoveryPort { get; set; }

        public int CommunicationPort { get; set; }

        public string NodeEndpoints { get; set; }

        public string StoragePath { get; set; }

        public bool EnablePersistence { get; set; }

        public IgniteCacheNameConfiguration CacheNames { get; set; }

        public IgniteTlsConfiguration TlsConfiguration { get; set; }
    }
}
