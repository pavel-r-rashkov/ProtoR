namespace ProtoR.Web.Infrastructure.Swagger
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Microsoft.OpenApi.Models;
    using ProtoR.Web.Controllers;
    using ProtoR.Web.Infrastructure.Identity;
    using ProtoR.Web.Resources;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class CommonResponseOperationFilter : IOperationFilter
    {
        private readonly AuthenticationConfiguration authenticationConfiguration;

        public CommonResponseOperationFilter(IOptions<AuthenticationConfiguration> authenticationConfiguration)
        {
            this.authenticationConfiguration = authenticationConfiguration.Value;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (this.authenticationConfiguration.AuthenticationEnabled)
            {
                this.AddUnauthroizedResponse(operation, context);
                this.AddForbiddenResponse(operation, context);
            }

            this.AddUnprocessableEntityResponse(operation, context);
        }

        private void AddUnauthroizedResponse(OpenApiOperation operation, OperationFilterContext context)
        {
            var authorizeAttribute = context.MethodInfo.DeclaringType
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .FirstOrDefault();

            if (authorizeAttribute == null)
            {
                return;
            }

            var unauthorizedCode = ((int)HttpStatusCode.Unauthorized).ToString(CultureInfo.InvariantCulture);
            operation.Responses.TryAdd(
                unauthorizedCode,
                new OpenApiResponse
                {
                    Description = "User or client is not authenticated.",
                });

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "OIDC",
                            },
                        }

                    ] = new[] { "protor-api" },
                },
            };
        }

        private void AddForbiddenResponse(OpenApiOperation operation, OperationFilterContext context)
        {
            var permissionAttribute = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<PermissionClaimAttribute>()
                .FirstOrDefault();

            if (permissionAttribute == null)
            {
                return;
            }

            var description = $"\"{permissionAttribute.Permission}\" permission is missing";

            if (context.MethodInfo.DeclaringType == typeof(GroupsController)
                || context.MethodInfo.DeclaringType == typeof(ConfigurationsController))
            {
                description += " or user/client doesn't have access to this group.";
            }
            else
            {
                description += ".";
            }

            var forbiddenCode = ((int)HttpStatusCode.Forbidden).ToString(CultureInfo.InvariantCulture);
            operation.Responses.TryAdd(
                forbiddenCode,
                new OpenApiResponse
                {
                    Description = description,
                });
        }

        private void AddUnprocessableEntityResponse(OpenApiOperation operation, OperationFilterContext context)
        {
            var isPostOrPut = context.MethodInfo
                .GetCustomAttributes(true)
                .Any(a => a.GetType() == typeof(HttpPostAttribute)
                    || a.GetType() == typeof(HttpPutAttribute));

            if (!isPostOrPut)
            {
                return;
            }

            var unprocessableEntityCode = ((int)HttpStatusCode.UnprocessableEntity).ToString(CultureInfo.InvariantCulture);
            var mediaType = new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Reference = new OpenApiReference
                    {
                        Id = nameof(ErrorModel),
                        Type = ReferenceType.Schema,
                    },
                },
            };

            operation.Responses.TryAdd(
                unprocessableEntityCode,
                new OpenApiResponse
                {
                    Description = "Invalid request data.",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        { "application/json", mediaType },
                        { "text/plain", mediaType },
                        { "text/json", mediaType },
                    },
                });
        }
    }
}
