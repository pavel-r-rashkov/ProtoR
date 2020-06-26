namespace ProtoR.Application.Client
{
    using System.Collections.Generic;
    using MediatR;

    public class CreateClientCommand : IRequest<long>
    {
        public string ClientId { get; set; }

        public string ClientName { get; set; }

        public string Secret { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<string> GrantTypes { get; set; }

        public IEnumerable<string> RedirectUris { get; set; }

        public IEnumerable<string> PostLogoutRedirectUris { get; set; }

        public IEnumerable<string> AllowedCorsOrigins { get; set; }

        public IEnumerable<long> RoleBindings { get; set; }

        public IEnumerable<string> GroupRestrictions { get; set; }
    }
}
