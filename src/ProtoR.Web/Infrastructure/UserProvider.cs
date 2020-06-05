namespace ProtoR.Web.Infrastructure
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using ProtoR.Application;
    using ProtoR.Web.Infrastructure.Identity;

    public class UserProvider : IUserProvider
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IOptions<AuthenticationConfiguration> authenticationConfiguration;

        public UserProvider(
            IHttpContextAccessor httpContextAccessor,
            IOptions<AuthenticationConfiguration> authenticationConfiguration)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.authenticationConfiguration = authenticationConfiguration;
        }

        public string GetCurrentUserName()
        {
            string userName = null;

            if (this.httpContextAccessor.HttpContext == null)
            {
                return userName;
            }

            var userNameClaim = this.httpContextAccessor.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == CustomClaim.UserNameClaimType);

            userName = userNameClaim?.Value ?? userName;

            if (string.IsNullOrEmpty(userName))
            {
                var clientClaim = this.httpContextAccessor.HttpContext.User.Claims
                    .FirstOrDefault(c => c.Type == "client_id");

                userName = clientClaim?.Value ?? userName;
            }

            return userName;
        }

        public IEnumerable<long> GetCategoryRestrictions()
        {
            if (!this.authenticationConfiguration.Value.AuthenticationEnabled)
            {
                return null;
            }

            return this.httpContextAccessor.HttpContext.User.Claims
                .Where(c => c.Type == CustomClaim.CategoryClaimType)
                .Select(c => long.Parse(c.Value, CultureInfo.InvariantCulture));
        }
    }
}
