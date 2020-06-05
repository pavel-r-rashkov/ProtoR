namespace ProtoR.Web.Resources.ClientResource
{
    using System.Collections.Generic;
    using ProtoR.Web.Infrastructure.Swagger;

    public class ClientWriteModel
    {
        [SwaggerExclude]
        public long Id { get; set; }

        public string ClientId { get; set; }

        public string ClientName { get; set; }

        public string Secret { get; set; }

        public IEnumerable<string> GrantTypes { get; set; }

        public IEnumerable<string> RedirectUris { get; set; }

        public IEnumerable<string> PostLogoutRedirectUris { get; set; }

        public IEnumerable<string> AllowedCorsOrigins { get; set; }

        public IEnumerable<long> RoleBindings { get; set; }

        public IEnumerable<long> CategoryBindings { get; set; }
    }
}
