namespace ProtoR.Web.Infrastructure.Identity
{
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Options;

    public class ProtoRUserRequirementHandler : AuthorizationHandler<ProtoRUserRequirement>
    {
        private readonly bool authenticationEnabled;

        public ProtoRUserRequirementHandler(IOptions<AuthenticationConfiguration> authOptions)
        {
            this.authenticationEnabled = authOptions.Value.AuthenticationEnabled;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ProtoRUserRequirement requirement)
        {
            if (!this.authenticationEnabled || context.User.Identities.Any(u => u.IsAuthenticated))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
