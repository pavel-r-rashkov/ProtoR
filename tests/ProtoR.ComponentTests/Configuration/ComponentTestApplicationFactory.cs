namespace ProtoR.ComponentTests.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac;
    using IdentityServer4.Models;
    using MediatR;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using ProtoR.Application.Configuration;
    using ProtoR.Domain.CategoryAggregate;
    using ProtoR.Domain.ClientAggregate;
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.SeedWork;
    using ProtoR.Domain.UserAggregate;
    using ProtoR.Infrastructure.DataAccess;
    using ProtoR.Web;
    using ProtoR.Web.Infrastructure;
    using ProtoR.Web.Infrastructure.Identity;
    using Serilog;
    using Client = ProtoR.Domain.ClientAggregate.Client;
    using Permission = ProtoR.Domain.RoleAggregate.Permission;
    using Version = ProtoR.Domain.SchemaGroupAggregate.Schemas.Version;

    public class ComponentTestApplicationFactory : WebApplicationFactory<Startup>
    {
        public long NonGlobalConfigurationId { get; private set; }

        public long NonDefaultCategoryId { get; private set; }

        public long RoleId { get; private set; }

        public long UserId { get; private set; }

        public long ClientId { get; private set; }

        public async Task Seed()
        {
            var igniteFactory = this.Services.GetService(typeof(IIgniteFactory)) as IIgniteFactory;
            var igniteConfiguration = this.Services.GetService(typeof(IIgniteConfiguration)) as IIgniteConfiguration;
            var ignite = igniteFactory.Instance();

            // Roles
            var roleRepository = this.Services.GetService(typeof(IRoleRepository)) as IRoleRepository;
            var adminRoleId = await roleRepository.Add(new Role(default, "Admin", "ADMIN", Enumeration.GetAll<Permission>()));
            this.RoleId = await roleRepository.Add(new Role(default, "Developer", "DEVELOPER", new Permission[] { Permission.GroupRead, Permission.SchemaRead }));

            // Categories
            var categoryRepository = this.Services.GetService(typeof(ICategoryRepository)) as ICategoryRepository;

            await categoryRepository.Add(Category.CreateDefault());

            await categoryRepository.Add(new Category("First Category"));
            this.NonDefaultCategoryId = await categoryRepository.Add(new Category("Second Category"));

            // Users
            var userRepository = this.Services.GetService(typeof(IUserRepository)) as IUserRepository;
            var user = new User(
                default,
                "TestUser",
                "TESTUSER",
                "abc123",
                new List<RoleBinding> { new RoleBinding(adminRoleId, default(int), null) },
                new List<CategoryBinding> { new CategoryBinding(this.NonDefaultCategoryId, default(int), null) });
            this.UserId = await userRepository.Add(user);

            // Clients
            var clientRepository = this.Services.GetService(typeof(IClientRepository)) as IClientRepository;
            var client = new Client(
                default,
                "client ID",
                "client name",
                "testsecret".Sha256(),
                new List<string> { "client_credentials" },
                new List<Uri>(),
                new List<Uri>(),
                new List<string>(),
                new List<RoleBinding> { new RoleBinding(adminRoleId, null, default(int)) },
                new List<CategoryBinding> { new CategoryBinding(this.NonDefaultCategoryId, null, default(int)) });
            this.ClientId = await clientRepository.Add(client);

            // Groups and schemas
            var groupRepository = this.Services.GetService(typeof(IProtoBufSchemaGroupRepository)) as IProtoBufSchemaGroupRepository;
            var schemas = new[]
            {
                new ProtoBufSchema(1, Version.Initial, "syntax = \"proto3\";"),
                new ProtoBufSchema(1, Version.Initial.Next(), "syntax = \"proto3\";"),
            };
            var group = new ProtoBufSchemaGroup(
                default,
                "Test Group",
                this.NonDefaultCategoryId,
                schemas);
            var groupId = await groupRepository.Add(group);

            // Configurations
            var configurationRepository = this.Services.GetService(typeof(IConfigurationRepository)) as IConfigurationRepository;

            var globalConfiguration = Configuration.DefaultGlobalConfiguration();
            await configurationRepository.Add(globalConfiguration);

            var rulesConfiguration = RuleFactory
                .GetProtoBufRules()
                .Select(r => new KeyValuePair<RuleCode, RuleConfiguration>(r.Code, new RuleConfiguration(false, Severity.Error)));

            var configuration = new Configuration(
                default,
                new Dictionary<RuleCode, RuleConfiguration>(rulesConfiguration),
                groupId,
                new GroupConfiguration(false, true, false, false));

            this.NonGlobalConfigurationId = await configurationRepository.Add(configuration);
        }

        public void Clear()
        {
            var igniteFactory = this.Services.GetService(typeof(IIgniteFactory)) as IIgniteFactory;
            var ignite = igniteFactory.Instance();
            var cacheNames = ignite.GetCacheNames();

            foreach (var cacheName in cacheNames)
            {
                var cache = ignite.GetCache<string, string>(cacheName);
                cache.Clear();
            }
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            return Host
                .CreateDefaultBuilder()
                .UseServiceProviderFactory(new TestAutofacServiceProviderFactory(this.OverrideContainerConfiguration))
                .ConfigureAppConfiguration(configuration =>
                {
                    configuration.AddInMemoryCollection(new[]
                    {
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.SchemaCacheName), "SchemaCacheName_ComponentTest"),
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.SchemaGroupCacheName), "SchemaGroupCacheName_ComponentTest"),
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.ConfigurationCacheName), "ConfigurationCacheName_ComponentTest"),
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.RuleConfigurationCacheName), "RuleConfigurationCacheName_ComponentTest"),
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.UserCacheName), "UserCacheName_ComponentTest"),
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.UserRoleCacheName), "UserRoleCacheName_ComponentTest"),
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.UserCategoryCacheName), "UserCategoryCacheName_ComponentTest"),
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.RoleCacheName), "RoleCacheName_ComponentTest"),
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.RolePermissionCacheName), "RolePermissionCacheName_ComponentTest"),
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.CategoryCacheName), "CategoryCacheName_ComponentTest"),
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.ClientCacheName), "ClientCacheName_ComponentTest"),
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.ClientRoleCacheName), "ClientRoleCacheName_ComponentTest"),
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.ClientCategoryCacheName), "ClientCategoryCacheName_ComponentTest"),
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.DiscoveryPort), "8000"),
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.CommunicationPort), "9000"),
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.NodeEndpoints), "127.0.0.1:8000"),
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.StoragePath), "/tmp/component-tests"),
                        new KeyValuePair<string, string>(nameof(IgniteConfiguration.EnablePersistence), "false"),
                        new KeyValuePair<string, string>(nameof(AuthenticationConfiguration.AuthenticationEnabled), "true"),
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
            // Override Autofac configuration
        }
    }
}
