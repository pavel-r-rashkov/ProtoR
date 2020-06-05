namespace ProtoR.Web.Infrastructure
{
    using Autofac;
    using Microsoft.Extensions.Configuration;
    using ProtoR.Application.Mapper;
    using ProtoR.Infrastructure.DataAccess;
    using ProtoR.Web.Infrastructure.Modules;

    public static class ContainerBuilderExtensions
    {
        public static void RegisterModules(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.RegisterModule(new MediatorModule());
            builder.RegisterModule(new CommonModule());
            builder.RegisterModule(new AutoMapperModule(typeof(Startup).Assembly, typeof(ApplicationProfile).Assembly));
            var igniteConfiguration = configuration.Get(typeof(IgniteConfiguration)) as IIgniteConfiguration;
            builder.RegisterModule(new IgniteModule(igniteConfiguration));
        }
    }
}
