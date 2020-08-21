namespace ProtoR.Web
{
    using Autofac;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using ProtoR.Web.Infrastructure;
    using ProtoR.Web.Infrastructure.Identity;
    using Serilog;

    public class Startup
    {
        public Startup(
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            this.Configuration = configuration;
            this.Environment = environment;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        public virtual void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModules(this.Configuration);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews(options =>
            {
                options.AddModelBinders();
            });
            services
                .AddControllers()
                .ConfigureCustomApiBehaviorOptions()
                .AddHybridModelBinder()
                .AddCustomFluentValidation();

            services
                .AddConfiguration(this.Configuration)
                .AddSwagger()
                .AddCustomIdentity()
                .AddCustomIdentityServer(this.Configuration)
                .AddCustomCors()
                .AddCustomAuthentication()
                .AddCustomHealthChecks();
        }

        public void Configure(
            IApplicationBuilder application,
            IOptions<AuthenticationConfiguration> authOptions,
            IOptions<TlsConfiguration> tlsConfiguration,
            ILogger logger)
        {
            application
                .UseIgnite()
                .UseCustomSwagger(this.Environment)
                .UseCustomExceptionHandler(logger)
                .UseCustomHttpsRedirection(tlsConfiguration)
                .UseRouting()
                .UseCors()
                .UseCustomAuthentication(authOptions)
                .UseCustomEndpoints();
        }
    }
}
