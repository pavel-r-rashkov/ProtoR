namespace ProtoR.ComponentTests.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using Autofac;
    using AutoFixture;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Moq;
    using ProtoR.Application.Configuration;
    using ProtoR.Application.Group;
    using ProtoR.Application.Schema;
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Web;
    using ProtoR.Web.Infrastructure;
    using Serilog;

    public class ComponentTestApplicationFactory : WebApplicationFactory<Startup>
    {
        private readonly Fixture fixture = new Fixture();

        public ComponentTestApplicationFactory()
        {
            this.GroupRepositoryMock = new Mock<IProtoBufSchemaGroupRepository>();
            this.ConfigurationRepositoryMock = new Mock<IConfigurationRepository>();
            this.SchemaDataProviderMock = new Mock<ISchemaDataProvider>();
            this.GroupDataProviderMock = new Mock<IGroupDataProvider>();
            this.ConfigurationDataProviderMock = new Mock<IConfigurationDataProvider>();
            this.ResetMockSetup();
        }

        public Mock<IProtoBufSchemaGroupRepository> GroupRepositoryMock { get; }

        public Mock<IConfigurationRepository> ConfigurationRepositoryMock { get; }

        public Mock<ISchemaDataProvider> SchemaDataProviderMock { get; }

        public Mock<IGroupDataProvider> GroupDataProviderMock { get; }

        public Mock<IConfigurationDataProvider> ConfigurationDataProviderMock { get; }

        public void ResetMockSetup()
        {
            // Group repository
            this.GroupRepositoryMock.Reset();

            var schemas = new[]
            {
                new ProtoBufSchema(1, Version.Initial, "syntax = \"proto3\";"),
                new ProtoBufSchema(1, Version.Initial.Next(), "syntax = \"proto3\";"),
            };
            this.GroupRepositoryMock
                .Setup(r => r.GetByName(It.IsAny<string>()))
                .ReturnsAsync(new ProtoBufSchemaGroup(
                    1,
                    "Test group name",
                    schemas));

            // Configuration repository
            this.ConfigurationRepositoryMock.Reset();

            var rulesConfiguration = RuleFactory
                .GetProtoBufRules()
                .Select(r => new KeyValuePair<RuleCode, RuleConfiguration>(r.Code, new RuleConfiguration(false, Severity.Error)));

            var configuration = new Configuration(
                1,
                new Dictionary<RuleCode, RuleConfiguration>(rulesConfiguration),
                1,
                new GroupConfiguration(false, true, false, false));

            this.ConfigurationRepositoryMock
                .Setup(r => r.GetById(It.IsAny<long>()))
                .ReturnsAsync(configuration);

            this.ConfigurationRepositoryMock
                .Setup(r => r.GetBySchemaGroupId(It.IsAny<long?>()))
                .ReturnsAsync(configuration);

            // Schema data provider
            this.SchemaDataProviderMock
                .Setup(d => d.GetByVersion(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(this.fixture.Create<SchemaDto>());

            this.SchemaDataProviderMock
                .Setup(d => d.GetLatestVersion(It.IsAny<string>()))
                .ReturnsAsync(this.fixture.Create<SchemaDto>());

            this.SchemaDataProviderMock
                .Setup(d => d.GetGroupSchemas(It.IsAny<string>()))
                .ReturnsAsync(this.fixture.CreateMany<SchemaDto>());

            // Group data provider
            this.GroupDataProviderMock
                .Setup(d => d.GetByName(It.IsAny<string>()))
                .ReturnsAsync(this.fixture.Create<GroupDto>());

            this.GroupDataProviderMock
                .Setup(d => d.GetGroups())
                .ReturnsAsync(this.fixture.CreateMany<GroupDto>());

            // Configuration data provider
            this.ConfigurationDataProviderMock
                .Setup(d => d.GetById(It.IsAny<long>()))
                .ReturnsAsync(this.fixture.Create<ConfigurationDto>());

            this.ConfigurationDataProviderMock
                .Setup(d => d.GetGlobalConfig())
                .ReturnsAsync(this.fixture.Create<ConfigurationDto>());

            this.ConfigurationDataProviderMock
                .Setup(d => d.GetConfigByGroupName(It.IsAny<string>()))
                .ReturnsAsync(this.fixture.Create<ConfigurationDto>());
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            return Host
                .CreateDefaultBuilder()
                .UseServiceProviderFactory(new TestAutofacServiceProviderFactory(this.OverrideContainerConfiguration))
                .ConfigureAppConfiguration(configuration =>
                {
                    // Fake configuration to satisfy IgniteConfiguration requirements
                    configuration.AddInMemoryCollection(new[]
                    {
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.SchemaCacheName), "SchemaCacheName"),
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.SchemaGroupCacheName), "SchemaGroupCacheName"),
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.ConfigurationCacheName), "ConfigurationCacheName"),
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.RuleConfigurationCacheName), "RuleConfigurationCacheName"),
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.DiscoveryPort), "8000"),
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.CommunicationPort), "9000"),
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.NodeEndpoints), "127.0.0.1:8000"),
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.StoragePath), "/"),
                    });
                })
                .ConfigureServices((hostBuilder, services) =>
                {
                })
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    webHostBuilder.UseStartup<Startup>();
                })
                .UseSerilog();
        }

        private void OverrideContainerConfiguration(ContainerBuilder builder)
        {
            builder.RegisterModule(new IgniteMockModule(
                this.GroupRepositoryMock,
                this.ConfigurationRepositoryMock,
                this.SchemaDataProviderMock,
                this.GroupDataProviderMock,
                this.ConfigurationDataProviderMock));
        }
    }
}
