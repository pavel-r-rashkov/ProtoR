namespace ProtoR.Web.Infrastructure.Modules
{
    using Apache.Ignite.Core;
    using Autofac;
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Infrastructure.DataAccess.Repositories;

    public class IgniteModule : Module
    {
        private readonly IIgnite ignite;

        public IgniteModule(IIgnite ignite)
        {
            this.ignite = ignite;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterInstance(this.ignite)
                .SingleInstance();

            builder
                .RegisterType<ProtoBufSchemaGroupRepository>()
                .As<IProtoBufSchemaGroupRepository>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<ConfigurationRepository>()
                .As<IConfigurationRepository>()
                .InstancePerLifetimeScope();
        }
    }
}
