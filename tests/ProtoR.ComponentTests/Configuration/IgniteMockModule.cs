namespace ProtoR.ComponentTests.Configuration
{
    using Autofac;
    using Moq;
    using ProtoR.Application.Configuration;
    using ProtoR.Application.Group;
    using ProtoR.Application.Schema;
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SeedWork;
    using ProtoR.Infrastructure.DataAccess;

    public class IgniteMockModule : Module
    {
        private readonly Mock<IProtoBufSchemaGroupRepository> groupRepositoryMock;
        private readonly Mock<IConfigurationRepository> configurationRepositoryMock;
        private readonly Mock<ISchemaDataProvider> schemaDataProviderMock;
        private readonly Mock<IGroupDataProvider> groupDataProviderMock;
        private readonly Mock<IConfigurationDataProvider> configurationDataProviderMock;

        public IgniteMockModule(
            Mock<IProtoBufSchemaGroupRepository> groupRepositoryMock,
            Mock<IConfigurationRepository> configurationRepositoryMock,
            Mock<ISchemaDataProvider> schemaDataProviderMock,
            Mock<IGroupDataProvider> groupDataProviderMock,
            Mock<IConfigurationDataProvider> configurationDataProviderMock)
        {
            this.groupRepositoryMock = groupRepositoryMock;
            this.configurationRepositoryMock = configurationRepositoryMock;
            this.schemaDataProviderMock = schemaDataProviderMock;
            this.groupDataProviderMock = groupDataProviderMock;
            this.configurationDataProviderMock = configurationDataProviderMock;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var igniteFactoryMock = new Mock<IIgniteFactory>();
            builder
                .RegisterInstance(igniteFactoryMock.Object)
                .As<IIgniteFactory>()
                .SingleInstance();

            var unitOfWorkMock = new Mock<IUnitOfWork>();
            builder
                .RegisterInstance(unitOfWorkMock.Object)
                .As<IUnitOfWork>()
                .SingleInstance();

            builder
                .RegisterInstance(this.groupRepositoryMock.Object)
                .As<IProtoBufSchemaGroupRepository>()
                .SingleInstance();

            builder
                .RegisterInstance(this.configurationRepositoryMock.Object)
                .As<IConfigurationRepository>()
                .SingleInstance();

            builder
                .RegisterInstance(this.schemaDataProviderMock.Object)
                .As<ISchemaDataProvider>()
                .SingleInstance();

            builder
                .RegisterInstance(this.groupDataProviderMock.Object)
                .As<IGroupDataProvider>()
                .SingleInstance();

            builder
                .RegisterInstance(this.configurationDataProviderMock.Object)
                .As<IConfigurationDataProvider>()
                .SingleInstance();
        }
    }
}
