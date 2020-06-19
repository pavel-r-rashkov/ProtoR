namespace ProtoR.Infrastructure.DataAccess.CacheItems
{
    using System;

    public class UserCacheItem
    {
        public string UserName { get; set; }

        public string NormalizedUserName { get; set; }

        public string PasswordHash { get; set; }

        public string GroupRestrictions { get; set; }

        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
