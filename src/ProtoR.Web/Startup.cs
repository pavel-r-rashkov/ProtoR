namespace ProtoR.Web
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using FluentValidation.AspNetCore;
    using IdentityServer4.Models;
    using IdentityServer4.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Cors.Infrastructure;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;
    using Microsoft.OpenApi.Models;
    using ProtoR.Application.Mapper;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Domain.UserAggregate;
    using ProtoR.Infrastructure.DataAccess;
    using ProtoR.Web.Infrastructure;
    using ProtoR.Web.Infrastructure.Identity;
    using ProtoR.Web.Infrastructure.Modules;
    using ProtoR.Web.Infrastructure.Swagger;
    using Swashbuckle.AspNetCore.Swagger;

    public class Startup
    {
        public Startup(
            IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public ILifetimeScope AutofacContainer { get; private set; }

        public virtual void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new MediatorModule());
            builder.RegisterModule(new CommonModule());
            builder.RegisterModule(new AutoMapperModule(typeof(Startup).Assembly, typeof(ApplicationProfile).Assembly));
            var igniteConfiguration = this.Configuration.Get(typeof(IgniteConfiguration)) as IIgniteConfiguration;
            builder.RegisterModule(new IgniteModule(igniteConfiguration));
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services
                .AddControllers()
                .AddHybridModelBinder()
                .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.Configure<IgniteConfiguration>(this.Configuration);
            services.Configure<AuthenticationConfiguration>(this.Configuration);

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ProtoR API",
                    Version = "v1",
                });

                config.SchemaFilter<SwaggerExcludeFilter>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                config.IncludeXmlComments(xmlPath);
                config.AddFluentValidationRules();
            });

            var authOptions = this.Configuration.Get<AuthenticationConfiguration>();

            services
                .AddIdentity<User, Role>()
                .AddUserStore<UserStore>()
                .AddRoleStore<RoleStore>()
                .AddDefaultTokenProviders(); // TODO

            services.AddScoped<ICorsPolicyService, ClientStoreCorsPolicyService>();

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
                    context.Response.StatusCode = 403;
                    return Task.CompletedTask;
                };
            });

            services.AddScoped<ICorsPolicyProvider, ClientCorsPolicyProvider>();
        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IOptions<AuthenticationConfiguration> authOptions)
        {
            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            var igniteFactory = this.AutofacContainer.Resolve<IIgniteFactory>();
            igniteFactory.InitalizeIgnite();
            igniteFactory.SetAutoFacPlugin(this.AutofacContainer);

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Web App V1");
                    options.RoutePrefix = string.Empty;
                });

                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler(options =>
            {
                options.Run(async context =>
                {
                    context.Response.ContentType = "text/html";
                    var exceptionHandlerPathFeature =
                        context.Features.Get<IExceptionHandlerPathFeature>();

                    // TODO handle domain exceptions
                    if (exceptionHandlerPathFeature?.Error is FileNotFoundException)
                    {
                        await context.Response.WriteAsync("Concrete exception");
                    }

                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("Internal Server Error");
                });
            });

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors();

            if (authOptions.Value.AuthenticationEnabled)
            {
                app.UseIdentityServer();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
