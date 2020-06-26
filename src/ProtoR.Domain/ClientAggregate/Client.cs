namespace ProtoR.Domain.ClientAggregate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SeedWork;

    public class Client : Entity, IAggregateRoot
    {
        private List<RoleBinding> roleBindings;
        private IReadOnlyCollection<GroupRestriction> groupRestrictions;

        public Client(
            long id,
            string clientId,
            string clientName,
            string secret,
            bool isActive,
            IReadOnlyCollection<string> grantTypes,
            IReadOnlyCollection<Uri> redirectUris,
            IReadOnlyCollection<Uri> postLogoutRedirectUris,
            IReadOnlyCollection<string> allowedCorsOrigins,
            IReadOnlyCollection<GroupRestriction> groupRestrictions,
            IReadOnlyCollection<RoleBinding> roleBindings)
            : base(id)
        {
            this.ClientId = clientId;
            this.ClientName = clientName;
            this.Secret = secret;
            this.IsActive = isActive;
            this.GrantTypes = grantTypes;
            this.RedirectUris = redirectUris;
            this.PostLogoutRedirectUris = postLogoutRedirectUris;
            this.AllowedCorsOrigins = allowedCorsOrigins;
            this.GroupRestrictions = groupRestrictions;
            this.roleBindings = roleBindings.ToList();
        }

        public string ClientId { get; set; }

        public string ClientName { get; set; }

        public string Secret { get; set; }

        public bool IsActive { get; set; }

        public IReadOnlyCollection<string> GrantTypes { get; set; }

        public IReadOnlyCollection<Uri> RedirectUris { get; set; }

        public IReadOnlyCollection<Uri> PostLogoutRedirectUris { get; set; }

        public IReadOnlyCollection<string> AllowedCorsOrigins { get; set; }

        public IReadOnlyCollection<GroupRestriction> GroupRestrictions
        {
            get
            {
                return this.groupRestrictions;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(this.GroupRestrictions));
                }

                if (!value.Any())
                {
                    throw new ArgumentException(nameof(this.GroupRestrictions));
                }

                this.groupRestrictions = value;
            }
        }

        public IReadOnlyCollection<RoleBinding> RoleBindings { get => this.roleBindings; }

        public void SetRoles(IEnumerable<long> roles)
        {
            this.roleBindings = roles
                .Select(roleId => new RoleBinding(roleId, null, this.Id))
                .ToList();
        }
    }
}
