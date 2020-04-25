namespace ProtoR.Web.Infrastructure.Modules
{
    using Autofac;
    using ProtoR.Application.Configuration;
    using ProtoR.Application.Group;
    using ProtoR.Application.Schema;
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SeedWork;
    using ProtoR.Infrastructure.DataAccess;
    using ProtoR.Infrastructure.DataAccess.DataProviders;
    using ProtoR.Infrastructure.DataAccess.Repositories;

    public class IgniteModule : Module
    {
        private readonly IIgniteConfiguration igniteConfiguration;

        public IgniteModule(IIgniteConfiguration igniteConfiguration)
        {
            this.igniteConfiguration = igniteConfiguration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterType<IgniteFactory>()
                .As<IIgniteFactory>()
                .SingleInstance();

            builder
                .RegisterInstance(this.igniteConfiguration)
                .As<IIgniteConfiguration>()
                .SingleInstance();

            builder
                .RegisterType<IgniteUnitOfWork>()
                .As<IUnitOfWork>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<ProtoBufSchemaGroupRepository>()
                .As<IProtoBufSchemaGroupRepository>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<ConfigurationRepository>()
                .As<IConfigurationRepository>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<SchemaDataProvider>()
                .As<ISchemaDataProvider>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<GroupDataProvider>()
                .As<IGroupDataProvider>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<ConfigurationDataProvider>()
                .As<IConfigurationDataProvider>()
                .InstancePerLifetimeScope();
        }
    }
}
