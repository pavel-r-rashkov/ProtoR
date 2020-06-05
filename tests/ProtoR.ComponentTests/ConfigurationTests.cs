namespace ProtoR.ComponentTests
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Threading.Tasks;
    using ProtoR.ComponentTests.Configuration;
    using ProtoR.Web.Resources.ConfigurationResource;
    using Xunit;

    public class ConfigurationTests : TestBase
    {
        public ConfigurationTests(
            ComponentTestApplicationFactory applicationFactory,
            TestTokenProvider tokenProvider)
            : base(applicationFactory, tokenProvider)
        {
        }

        [Fact]
        public async Task GetGlobalConfiguration_ShouldReturn200Ok()
        {
            var id = "global";
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Configurations/{id}",
            };

            var response = await this.Client.GetAsync(uriBuilder.Uri);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetConfiguration_ShouldReturn200Ok()
        {
            var id = this.ApplicationFactory.NonGlobalConfigurationId.ToString(CultureInfo.InvariantCulture);
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Configurations/{id}",
            };

            var response = await this.Client.GetAsync(uriBuilder.Uri);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task PutConfiguration_WithValidGlobalConfiguration_ShouldReturn200Ok()
        {
            string id = "global";
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Configurations/{id}",
            };
            var configuration = new ConfigurationWriteModel
            {
                Transitive = false,
                BackwardCompatible = true,
                ForwardCompatible = false,
                Inherit = false,
                RuleConfigurations = new[]
                {
                    new RuleConfigurationModel
                    {
                        RuleCode = "PB0001",
                        Severity = 1,
                        Inherit = false,
                    },
                },
            };

            using var contents = new JsonHttpContent(configuration);
            var response = await this.Client.PutAsync(uriBuilder.Uri, contents);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task PutConfiguration_WithValidConfiguration_ShouldReturn200Ok()
        {
            string id = this.ApplicationFactory.NonGlobalConfigurationId.ToString(CultureInfo.InvariantCulture);
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Configurations/{id}",
            };
            var configuration = new ConfigurationWriteModel
            {
                Transitive = false,
                BackwardCompatible = true,
                ForwardCompatible = false,
                Inherit = false,
                RuleConfigurations = new[]
                {
                    new RuleConfigurationModel
                    {
                        RuleCode = "PB0001",
                        Severity = 1,
                        Inherit = false,
                    },
                },
            };

            using var contents = new JsonHttpContent(configuration);
            var response = await this.Client.PutAsync(uriBuilder.Uri, contents);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task PutConfiguration_WithInvalidConfiguration_ShouldReturn400BadRequest()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = "api/Configurations/1",
            };
            var configuration = new ConfigurationWriteModel
            {
                Transitive = false,
                BackwardCompatible = false,
                ForwardCompatible = false,
                Inherit = false,
                RuleConfigurations = new[]
                {
                    new RuleConfigurationModel
                    {
                        RuleCode = "PB0001",
                        Severity = 1,
                        Inherit = false,
                    },
                },
            };

            using var contents = new JsonHttpContent(configuration);
            var response = await this.Client.PutAsync(uriBuilder.Uri, contents);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
