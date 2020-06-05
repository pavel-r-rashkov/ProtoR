namespace ProtoR.Web.Resources.ClientResource
{
    using System;
    using System.Collections.Generic;

    public class ClientReadModel
    {
        public long Id { get; set; }

        public string ClientId { get; set; }

        public string ClientName { get; set; }

        public IEnumerable<string> GrantTypes { get; set; }

        public IEnumerable<string> RedirectUris { get; set; }

        public IEnumerable<string> PostLogoutRedirectUris { get; set; }

        public IEnumerable<string> AllowedCorsOrigins { get; set; }

        public IEnumerable<string> AllowedScopes { get; set; }

        public IEnumerable<long> RoleBindings { get; set; }

        public IEnumerable<long> CategoryBindings { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }
    }
}
