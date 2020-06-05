namespace ProtoR.Web.Infrastructure.Identity
{
    using System.Globalization;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Options;

    public class PermissionClaimFilter : IAuthorizationFilter
    {
        private readonly bool authEnabled;

        public PermissionClaimFilter(IOptions<AuthenticationConfiguration> options)
        {
            this.authEnabled = options.Value.AuthenticationEnabled;
        }

        public Permission Permission { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!this.authEnabled)
            {
                return;
            }

            var allowedClaimValue = ((int)this.Permission).ToString(CultureInfo.InvariantCulture);

            var hasPermission = context.HttpContext.User.Claims
                .Any(c => c.Type == CustomClaim.PermissionClaimType
                    && c.Value == allowedClaimValue);

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
