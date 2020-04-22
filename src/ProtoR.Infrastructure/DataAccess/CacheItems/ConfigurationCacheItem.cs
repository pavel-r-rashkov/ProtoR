namespace ProtoR.Infrastructure.DataAccess.CacheItems
{
    public class ConfigurationCacheItem
    {
        public long? SchemaGroupId { get; set; }

        public bool Inherit { get; set; }

        public bool ForwardCompatible { get; set; }

        public bool BackwardCompatible { get; set; }

        public bool Transitive { get; set; }
    }
}
