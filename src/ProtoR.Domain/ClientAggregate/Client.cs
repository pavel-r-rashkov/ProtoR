namespace ProtoR.Domain.ClientAggregate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ProtoR.Domain.CategoryAggregate;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Domain.SeedWork;

    public class Client : Entity, IAggregateRoot
    {
        private List<RoleBinding> roleBindings;
        private List<CategoryBinding> categoryBindings;

        public Client(
            long id,
            string clientId,
            string clientName,
            string secret,
            IReadOnlyCollection<string> grantTypes,
            IReadOnlyCollection<Uri> redirectUris,
            IReadOnlyCollection<Uri> postLogoutRedirectUris,
            IReadOnlyCollection<string> allowedCorsOrigins,
            IReadOnlyCollection<RoleBinding> roleBindings,
            IReadOnlyCollection<CategoryBinding> categoryBindings)
            : base(id)
        {
            this.ClientId = clientId;
            this.ClientName = clientName;
            this.Secret = secret;
            this.GrantTypes = grantTypes;
            this.RedirectUris = redirectUris;
            this.PostLogoutRedirectUris = postLogoutRedirectUris;
            this.AllowedCorsOrigins = allowedCorsOrigins;
            this.roleBindings = roleBindings.ToList();
            this.categoryBindings = categoryBindings.ToList();
        }

        public string ClientId { get; set; }

        public string ClientName { get; set; }

        public string Secret { get; set; }

        public IReadOnlyCollection<string> GrantTypes { get; set; }

        public IReadOnlyCollection<Uri> RedirectUris { get; set; }

        public IReadOnlyCollection<Uri> PostLogoutRedirectUris { get; set; }

        public IReadOnlyCollection<string> AllowedCorsOrigins { get; set; }

        public IReadOnlyCollection<RoleBinding> RoleBindings { get => this.roleBindings; }

        public IReadOnlyCollection<CategoryBinding> CategoryBindings { get => this.categoryBindings; }

        public void SetRoles(IEnumerable<long> roles)
        {
            this.roleBindings = roles
                .Select(roleId => new RoleBinding(roleId, null, this.Id))
                .ToList();
        }

        public void SetCategories(IEnumerable<long> categories)
        {
            this.categoryBindings = categories
                .Select(categoryId => new CategoryBinding(categoryId, null, this.Id))
                .ToList();
        }
    }
}
