namespace ProtoR.Web.Infrastructure.Modules
{
    using Apache.Ignite.Core;
    using Autofac;
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.ConfigurationSetAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
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
                .RegisterType<SchemaGroupRepository>()
                .As<ISchemaGroupRepository<ProtoBufSchema, FileDescriptorSet>>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<ConfigurationRepository>()
                .As<IConfigurationSetRepository>()
                .InstancePerLifetimeScope();
        }
    }
}
