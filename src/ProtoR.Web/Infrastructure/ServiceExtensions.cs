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
    using FluentValidation;
    using FluentValidation.AspNetCore;
    using IdentityServer4.Models;
    using IdentityServer4.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors.Infrastructure;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.DataProtection.KeyManagement;
    using Microsoft.AspNetCore.DataProtection.Repositories;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Options;
    using Microsoft.OpenApi.Models;
    using ProtoR.Application.Common;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Domain.UserAggregate;
    using ProtoR.Infrastructure.DataAccess;
    using ProtoR.Web.Infrastructure.Identity;
    using ProtoR.Web.Infrastructure.ModelBinders;
    using ProtoR.Web.Infrastructure.Swagger;
    using ProtoR.Web.Resources;
    using Swashbuckle.AspNetCore.Swagger;

    public static class ServiceExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            var authOptions = configuration
                .GetSection(nameof(AuthenticationConfiguration))
                .Get<AuthenticationConfiguration>();

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ProtoR API",
                    Version = "v1",
                });

                config.SchemaFilter<SwaggerExcludeFilter>();
                config.OperationFilter<HybridOperationFilter>();
                config.OperationFilter<IgnoreParameterPrefixFilter>();
                config.OperationFilter<CommonResponseOperationFilter>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                config.IncludeXmlComments(xmlPath);
                config.AddFluentValidationRules();

                if (authOptions.AuthenticationEnabled)
                {
                    config.AddSecurityDefinition("OIDC", new OpenApiSecurityScheme
                    {
                        // OIDC is not supported in Swagger UI.
                        // https://swagger.io/docs/specification/authentication/openid-connect-discovery/
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            ClientCredentials = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri("https://localhost:5001/connect/authorize"),
                                TokenUrl = new Uri("https://localhost:5001/connect/token"),
                                Scopes = new Dictionary<string, string>
                                {
                                    { "protor-api", "ProtoR api" },
                                },
                            },
                        },
                    });
                }
            });

            return services;
        }

        public static IServiceCollection AddCustomIdentityServer(this IServiceCollection services, IConfiguration configuration)
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
                .AddPersistedGrantStore<IgniteGrantStore>()
                .AddAspNetIdentity<User>()
                .AddProfileService<ProfileService>();

            var authOptions = configuration
                .GetSection(nameof(AuthenticationConfiguration))
                .Get<AuthenticationConfiguration>();

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
            services
                .Configure<IgniteExternalConfiguration>(configuration.GetSection("Ignite"))
                .Configure<AuthenticationConfiguration>(configuration.GetSection(nameof(AuthenticationConfiguration)))
                .Configure<TlsConfiguration>(configuration.GetSection(nameof(TlsConfiguration)));

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

                    return new UnprocessableEntityObjectResult(new ErrorModel
                    {
                        Errors = errors,
                        Message = "Invalid request data.",
                    });
                };
            });
        }

        public static MvcOptions AddModelBinders(this MvcOptions options)
        {
            options.ModelBinderProviders.Insert(0, new PaginationBinderProvider());
            options.ModelBinderProviders.Insert(0, new FilterBinderProvider());
            options.ModelBinderProviders.Insert(0, new SortOrderBinderProvider());

            return options;
        }

        public static IMvcBuilder AddCustomFluentValidation(this IMvcBuilder builder)
        {
            builder.AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<Startup>());

            ValidatorOptions.PropertyNameResolver = (type, member, expression) =>
            {
                if (member == null)
                {
                    return null;
                }

                if (member is PropertyInfo property
                    && property.PropertyType == typeof(Pagination))
                {
                    return string.Empty;
                }

                return member.Name;
            };

            return builder;
        }

        public static IHealthChecksBuilder AddCustomHealthChecks(this IServiceCollection services)
        {
            return services
                .AddHealthChecks()
                .AddCheck<IgniteHealthCheck>(IgniteHealthCheck.Name);
        }

        public static IServiceCollection AddCustomDataProtection(this IServiceCollection services)
        {
            services
                .AddScoped<IXmlRepository, IgniteKeyStore>()
                .TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<KeyManagementOptions>, PostConfigureKeyManagementOptions>());

            services
                .AddDataProtection()
                .SetApplicationName("ProtoR");

            return services;
        }

        public static IServiceCollection AddCustomAntiforgery(this IServiceCollection services)
        {
            return services.AddAntiforgery(antiforgeryOptions =>
            {
                antiforgeryOptions.Cookie.Name = "XSRF-TOKEN";
                antiforgeryOptions.HeaderName = "X-XSRF-TOKEN";
                antiforgeryOptions.SuppressXFrameOptionsHeader = true;
            });
        }
    }
}
