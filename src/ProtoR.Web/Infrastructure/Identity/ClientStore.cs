namespace ProtoR.Web.Infrastructure.Identity
{
    using System.Linq;
    using System.Threading.Tasks;
    using IdentityServer4;
    using IdentityServer4.Models;
    using IdentityServer4.Stores;
    using ProtoR.Domain.ClientAggregate;
    using ProtoR.Domain.RoleAggregate;
    using Client = IdentityServer4.Models.Client;

    public class ClientStore : IClientStore
    {
        private readonly IClientRepository clientRepository;
        private readonly IRoleRepository roleRepository;

        public ClientStore(
            IClientRepository clientRepository,
            IRoleRepository roleRepository)
        {
            this.clientRepository = clientRepository;
            this.roleRepository = roleRepository;
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var dbClient = await this.clientRepository.GetByClientId(clientId);

            if (dbClient == null || !dbClient.IsActive)
            {
                return null;
            }

            var client = new Client
            {
                ClientId = dbClient.ClientId,
                ClientName = dbClient.ClientName,
                ClientSecrets = { new Secret(dbClient.Secret) },
                AllowedGrantTypes = dbClient.GrantTypes.ToList(),
                AllowedScopes = { "protor-api" },
                RequireConsent = false,
                AllowOfflineAccess = true,
                RequirePkce = false,
                RequireClientSecret = false,
                RedirectUris = dbClient.RedirectUris.Select(u => u.ToString()).ToList(),
                PostLogoutRedirectUris = dbClient.PostLogoutRedirectUris.Select(u => u.ToString()).ToList(),
                AllowedCorsOrigins = dbClient.AllowedCorsOrigins.ToList(),
                ClientClaimsPrefix = string.Empty,
            };

            if (!client.AllowedGrantTypes.Contains(GrantTypes.ClientCredentials.First()))
            {
                client.AllowedScopes.Add(IdentityServerConstants.StandardScopes.OpenId);
            }

            await this.AddCustomClaims(client, dbClient);

            return client;
        }

        private async Task AddCustomClaims(Client client, Domain.ClientAggregate.Client dbClient)
        {
            var roles = await this.roleRepository.GetRoles(dbClient.RoleBindings.Select(r => r.RoleId));
            var permissions = roles.SelectMany(r => r.Permissions).Select(p => p.Id).Distinct();
            var groupRestrictions = dbClient.GroupRestrictions.Select(gr => gr.Pattern);

            foreach (var claim in CustomClaim.ForPermissions(permissions))
            {
                client.Claims.Add(claim);
            }

            foreach (var claim in CustomClaim.ForGroupRestrictions(groupRestrictions))
            {
                client.Claims.Add(claim);
            }

            client.Claims.Add(CustomClaim.ForClientName(dbClient.ClientName));
        }
    }
}
