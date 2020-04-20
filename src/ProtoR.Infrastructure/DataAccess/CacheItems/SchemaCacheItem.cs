namespace ProtoR.Infrastructure.DataAccess.CacheItems
{
    using System;

    public class SchemaCacheItem
    {
        public long SchemaGroupId { get; set; }

        public int Version { get; set; }

        public string Contents { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
