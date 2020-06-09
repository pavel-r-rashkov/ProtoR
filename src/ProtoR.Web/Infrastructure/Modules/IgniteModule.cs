namespace ProtoR.Web.Infrastructure.Modules
{
    using Autofac;
    using ProtoR.Application.Category;
    using ProtoR.Application.Client;
    using ProtoR.Application.Configuration;
    using ProtoR.Application.Group;
    using ProtoR.Application.Role;
    using ProtoR.Application.Schema;
    using ProtoR.Application.User;
    using ProtoR.Domain.CategoryAggregate;
    using ProtoR.Domain.ClientAggregate;
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SeedWork;
    using ProtoR.Domain.UserAggregate;
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

            builder.Register(d =>
                {
                    var igniteFactory = d.Resolve<IIgniteFactory>();

                    return igniteFactory
                        .Instance()
                        .GetServices()
                        .GetService<IClusterSingletonService>(nameof(IClusterSingletonService));
                })
                .As<IClusterSingletonService>();

            builder
                .RegisterInstance(this.igniteConfiguration)
                .As<IIgniteConfiguration>()
                .SingleInstance();

            builder
                .RegisterType<IgniteUnitOfWork>()
                .As<IUnitOfWork>()
                .InstancePerDependency();

            builder
                .RegisterType<ProtoBufSchemaGroupRepository>()
                .As<IProtoBufSchemaGroupRepository>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<ConfigurationRepository>()
                .As<IConfigurationRepository>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<RoleRepository>()
                .As<IRoleRepository>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<CategoryRepository>()
                .As<ICategoryRepository>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<UserRepository>()
                .As<IUserRepository>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<ClientRepository>()
                .As<IClientRepository>()
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

            builder
                .RegisterType<RoleDataProvider>()
                .As<IRoleDataProvider>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<CategoryDataProvider>()
                .As<ICategoryDataProvider>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<UserDataProvider>()
                .As<IUserDataProvider>()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<ClientDataProvider>()
                .As<IClientDataProvider>()
                .InstancePerLifetimeScope();
        }
    }
}
