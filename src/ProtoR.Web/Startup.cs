namespace Web
{
    using System;
    using System.IO;
    using System.Reflection;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using FluentValidation.AspNetCore;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;
    using ProtoR.Infrastructure.DataAccess;
    using ProtoR.Web.Infrastructure;
    using ProtoR.Web.Infrastructure.Modules;
    using ProtoR.Web.Infrastructure.Swagger;
    using Swashbuckle.AspNetCore.Swagger;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public ILifetimeScope AutofacContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddHybridModelBinder()
                .AddFluentValidation(config => config.RegisterValidatorsFromAssemblyContaining<Startup>());

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
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new MediatorModule());
            builder.RegisterModule(new CommonModule());
            builder.RegisterModule(new AutoMapperModule(typeof(Startup).Assembly));

            var igniteFactory = new IgniteFactory(this.GetIgniteConfiguration());
            builder.RegisterModule(new IgniteModule(igniteFactory.InitalizeIgnite()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Web App V1");
                options.RoutePrefix = string.Empty;
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private IIgniteConfigurationProvider GetIgniteConfiguration()
        {
            return new IgniteConfigurationProvider()
            {
                SchemaCacheName = "PROTOR_SCHEMA_CACHE",
                SchemaGroupCacheName = "PROTOR_SCHEMA_GROUP_CACHE",
                ConfigurationCacheName = "PROTOR_CONFIGURATION_CACHE",
                RuleConfigurationCacheName = "PROTOR_RULE_CONFIGURATION_CACHE",
                DiscoveryPort = this.Configuration.GetValue<int>("PROTOR_DISCOVERY_PORT"),
                CommunicationPort = this.Configuration.GetValue<int>("PROTOR_COMMUNICATION_PORT"),
                NodeEndpoints = this.Configuration.GetValue<string>("PROTOR_NODE_ENPOINTS").Split(',', StringSplitOptions.RemoveEmptyEntries),
                StoragePath = this.Configuration.GetValue<string>("PROTOR_STORAGE_PATH"),
            };
        }
    }
}
