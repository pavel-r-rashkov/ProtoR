namespace ProtoR.Infrastructure.DataAccess.CacheItems
{
    using System;

    public class GrantCacheItem
    {
        public string ClientId { get; set; }

        public DateTime CreationTime { get; set; }

        public string Data { get; set; }

        public DateTime? Expiration { get; set; }

        public string SubjectId { get; set; }

        public string Type { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
