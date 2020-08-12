namespace ProtoR.Web
{
    using Autofac;
    using FluentValidation.AspNetCore;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using ProtoR.Web.Infrastructure;
    using ProtoR.Web.Infrastructure.Identity;
    using ProtoR.Web.Infrastructure.ModelBinders;

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
                .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<Startup>());

            var authOptions = this.Configuration.Get<AuthenticationConfiguration>();
            services
                .AddConfiguration(this.Configuration)
                .AddSwagger()
                .AddCustomIdentity()
                .AddCustomIdentityServer(authOptions)
                .AddCustomCors()
                .AddCustomAuthentication()
                .ConfigureFluentValidation();
        }

        public void Configure(
            IApplicationBuilder application,
            IOptions<AuthenticationConfiguration> authOptions)
        {
            application
                .UseIgnite()
                .UseCustomSwagger(this.Environment)
                .UseCustomExceptionHandler()
                .UseHttpsRedirection()
                .UseRouting()
                .UseCors()
                .UseCustomAuthentication(authOptions)
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapDefaultControllerRoute();
                });
        }
    }
}
