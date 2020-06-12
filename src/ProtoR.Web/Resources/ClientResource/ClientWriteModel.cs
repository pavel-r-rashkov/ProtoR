namespace ProtoR.Web.Resources.ClientResource
{
    using System.Collections.Generic;
    using ProtoR.Web.Infrastructure.Swagger;

    /// <summary>
    /// Client resource.
    /// </summary>
    public class ClientWriteModel
    {
        /// <summary>
        /// Client ID.
        /// </summary>
        [SwaggerExclude]
        public long Id { get; set; }

        /// <summary>
        /// Client ID (Open ID).
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Client display name.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Secret.
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// client_credentials or authorization_code.
        /// </summary>
        public IEnumerable<string> GrantTypes { get; set; }

        /// <summary>
        /// White list of URIs to be used after login. Not needed for client_credentials grant.
        /// </summary>
        public IEnumerable<string> RedirectUris { get; set; }

        /// <summary>
        /// White list of URIs to be used after logout. Not needed for client_credentials grant.
        /// </summary>
        public IEnumerable<string> PostLogoutRedirectUris { get; set; }

        /// <summary>
        /// Allowed client origins. Not needed for client_credentials grant.
        /// </summary>
        public IEnumerable<string> AllowedCorsOrigins { get; set; }

        /// <summary>
        /// Role IDs associated with this client.
        /// </summary>
        public IEnumerable<long> RoleBindings { get; set; }

        /// <summary>
        /// Category IDs associated with this client.
        /// </summary>
        public IEnumerable<long> CategoryBindings { get; set; }
    }
}
