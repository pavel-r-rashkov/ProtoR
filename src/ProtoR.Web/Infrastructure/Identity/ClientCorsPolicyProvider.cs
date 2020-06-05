namespace ProtoR.Web.Infrastructure.Identity
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Cors.Infrastructure;
    using Microsoft.AspNetCore.Http;
    using ProtoR.Application.Client;

    public class ClientCorsPolicyProvider : ICorsPolicyProvider
    {
        private readonly IClientDataProvider clientData;
        private readonly CorsPolicyBuilder builder;

        public ClientCorsPolicyProvider(IClientDataProvider clientData)
        {
            this.clientData = clientData;
            this.builder = new CorsPolicyBuilder();
        }

        public async Task<CorsPolicy> GetPolicyAsync(HttpContext context, string policyName)
        {
            var origins = await this.clientData.GetOrigins();

            var policy = this.builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithOrigins(origins.ToArray())
                .Build();

            return policy;
        }
    }
}
