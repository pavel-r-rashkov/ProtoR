namespace ProtoR.Infrastructure.DataAccess.CacheItems
{
    using System;

    public class ClientCacheItem
    {
        public string ClientId { get; set; }

        public string ClientName { get; set; }

        public string Secret { get; set; }

        public string GrantTypes { get; set; }

        public string RedirectUris { get; set; }

        public string PostLogoutRedirectUris { get; set; }

        public string AllowedCorsOrigins { get; set; }

        public string GroupRestrictions { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
