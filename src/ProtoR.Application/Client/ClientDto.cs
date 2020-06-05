namespace ProtoR.Application.Client
{
    using System;
    using System.Collections.Generic;

    public class ClientDto
    {
        public long Id { get; set; }

        public string ClientId { get; set; }

        public string ClientName { get; set; }

        public IEnumerable<string> GrantTypes { get; set; }

        public IEnumerable<string> RedirectUris { get; set; }

        public IEnumerable<string> PostLogoutRedirectUris { get; set; }

        public IEnumerable<string> AllowedCorsOrigins { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public IEnumerable<long> RoleBindings { get; set; }

        public IEnumerable<long> CategoryBindings { get; set; }
    }
}
