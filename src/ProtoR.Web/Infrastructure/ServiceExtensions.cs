namespace ProtoR.Web.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using IdentityServer4.Models;
    using IdentityServer4.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors.Infrastructure;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OpenApi.Models;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Domain.UserAggregate;
    using ProtoR.Infrastructure.DataAccess;
    using ProtoR.Web.Infrastructure.Identity;
    using ProtoR.Web.Infrastructure.Swagger;
    using ProtoR.Web.Resources;
    using Swashbuckle.AspNetCore.Swagger;

    public static class ServiceExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ProtoR API",
                    Version = "v1",
                });

                config.SchemaFilter<SwaggerExcludeFilter>();
                config.OperationFilter<HybridOperationFilter>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                config.IncludeXmlComments(xmlPath);
                config.AddFluentValidationRules();
            });

            return services;
        }

        public static IServiceCollection AddCustomIdentityServer(this IServiceCollection services, AuthenticationConfiguration authOptions)
        {
            var builder = services
                .AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })
                .AddInMemoryIdentityResources(new List<IdentityResource>
                {
                    new IdentityResources.OpenId(),
                })
                .AddInMemoryApiResources(new List<ApiResource>
                {
                    new ApiResource("protor-api", "ProtoR API") { },
                })
                .AddClientStore<ClientStore>()
                .AddAspNetIdentity<User>()
                .AddProfileService<ProfileService>();

            if (authOptions.AuthenticationEnabled)
            {
                if (string.IsNullOrEmpty(authOptions.SigningCredentialPath))
                {
                    builder.AddDeveloperSigningCredential();
                }
                else
                {
                    var signingCredentials = new X509Certificate2(authOptions.SigningCredentialPath, authOptions.SigningCredentialPassword);
                    builder.AddSigningCredential(signingCredentials);
                }
            }

            return services;
        }

        public static IServiceCollection AddCustomCors(this IServiceCollection services)
        {
            services.AddScoped<ICorsPolicyService, ClientStoreCorsPolicyService>();
            services.AddScoped<ICorsPolicyProvider, ClientCorsPolicyProvider>();

            return services;
        }

        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication().AddLocalApi("protor-api", options =>
            {
                options.ExpectedScope = "protor-api";
            });
            services.AddScoped(typeof(PermissionClaimFilter));

            services.AddScoped<IAuthorizationHandler, ProtoRUserRequirementHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ProtoRClientPolicy", policy =>
                {
                    policy.AddRequirements(new ProtoRUserRequirement());
                    policy.AddAuthenticationSchemes("protor-api");
                });
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return Task.CompletedTask;
                };
            });

            return services;
        }

        public static IServiceCollection AddCustomIdentity(this IServiceCollection services)
        {
            services
                .AddIdentity<User, Role>()
                .AddUserStore<UserStore>()
                .AddRoleStore<RoleStore>()
                .AddDefaultTokenProviders(); // TODO

            return services;
        }

        public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<IgniteExternalConfiguration>(configuration);
            services.Configure<AuthenticationConfiguration>(configuration);

            return services;
        }

        public static IMvcBuilder ConfigureCustomApiBehaviorOptions(this IMvcBuilder builder)
        {
            return builder.ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState.Keys
                        .Select(errorKey => new FieldErrorModel
                        {
                            Field = errorKey.ToCamelCase(),
                            ErrorMessages = context.ModelState[errorKey].Errors.Select(error => error.ErrorMessage),
                        });

                    return new BadRequestObjectResult(new ErrorModel
                    {
                        Errors = errors,
                        Message = "Invalid request data.",
                    });
                };
            });
        }
    }
}
