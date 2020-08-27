namespace ProtoR.Web.Infrastructure.Identity
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Antiforgery;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.Extensions.Options;
    using Serilog;

    public class ConditionalAntiforgeryFilter : IAsyncAuthorizationFilter, IAntiforgeryPolicy
    {
        private readonly IAntiforgery antiforgery;
        private readonly IOptions<AuthenticationConfiguration> authenticationConfiguration;
        private readonly ILogger logger;

        public ConditionalAntiforgeryFilter(
            IAntiforgery antiforgery,
            IOptions<AuthenticationConfiguration> authenticationConfiguration,
            ILogger logger)
        {
            this.antiforgery = antiforgery;
            this.authenticationConfiguration = authenticationConfiguration;
            this.logger = logger;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!context.IsEffectivePolicy<IAntiforgeryPolicy>(this))
            {
                return;
            }

            if (!this.authenticationConfiguration.Value.AuthenticationEnabled)
            {
                return;
            }

            var userNameClaim = context.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == CustomClaim.UserNameClaimType);

            if (userNameClaim == null)
            {
                return;
            }

            var requestMethod = context.HttpContext.Request.Method;

            if (HttpMethods.IsPost(requestMethod)
                || HttpMethods.IsPut(requestMethod)
                || HttpMethods.IsPatch(requestMethod)
                || HttpMethods.IsDelete(requestMethod))
            {
                try
                {
                    await this.antiforgery.ValidateRequestAsync(context.HttpContext);
                }
                catch (AntiforgeryValidationException exception)
                {
                    context.Result = new AntiforgeryValidationFailedResult();
                    this.logger.Warning(exception, "Request failed antiforgery token validation");
                }
            }
        }
    }
}
