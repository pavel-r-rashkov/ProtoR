namespace ProtoR.Infrastructure.DataAccess.CacheItems
{
    using System;

    public class RoleCacheItem
    {
        public string Name { get; set; }

        public string NormalizedName { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
