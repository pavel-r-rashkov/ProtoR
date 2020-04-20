namespace ProtoR.Infrastructure.DataAccess.CacheItems
{
    using System;
    using Apache.Ignite.Core.Cache.Configuration;

    public class SchemaGroupCacheItem
    {
        public string Name { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
