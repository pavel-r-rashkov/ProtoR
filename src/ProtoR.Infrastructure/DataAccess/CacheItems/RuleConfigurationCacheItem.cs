namespace ProtoR.Infrastructure.DataAccess.CacheItems
{
    public class RuleConfigurationCacheItem
    {
        public long ConfigurationId { get; set; }

        public string RuleCode { get; set; }

        public bool Inherit { get; set; }

        public int Severity { get; set; }
    }
}
