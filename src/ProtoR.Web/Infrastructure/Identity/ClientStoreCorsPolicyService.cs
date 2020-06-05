namespace ProtoR.Web.Infrastructure.Identity
{
    using System.Linq;
    using System.Threading.Tasks;
    using IdentityServer4.Services;
    using ProtoR.Application.Client;

    public class ClientStoreCorsPolicyService : ICorsPolicyService
    {
        private readonly IClientDataProvider clientData;

        public ClientStoreCorsPolicyService(IClientDataProvider clientData)
        {
            this.clientData = clientData;
        }

        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            var allowedOrigins = await this.clientData.GetOrigins();

            return allowedOrigins.Contains(origin);
        }
    }
}
