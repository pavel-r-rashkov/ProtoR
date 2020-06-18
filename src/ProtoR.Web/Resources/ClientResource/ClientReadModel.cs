namespace ProtoR.Web.Resources.ClientResource
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Client resource.
    /// </summary>
    public class ClientReadModel : ICreationInfo
    {
        /// <summary>
        /// Client ID.
        /// </summary>
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
        /// Group restriction patterns associated with this client.
        /// </summary>
        public IEnumerable<string> GroupRestrictions { get; set; }

        /// <inheritdoc cref="ICreationInfo" />
        public DateTime CreatedOn { get; set; }

        /// <inheritdoc cref="ICreationInfo" />
        public string CreatedBy { get; set; }
    }
}
