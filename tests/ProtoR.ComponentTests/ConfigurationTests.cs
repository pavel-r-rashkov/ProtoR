namespace ProtoR.ComponentTests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using ProtoR.ComponentTests.Configuration;
    using ProtoR.Web.Resources.ConfigurationResource;
    using Xunit;

    [Collection(CollectionNames.TestApplicationCollection)]
    public sealed class ConfigurationTests : IDisposable
    {
        private readonly HttpClient client;

        public ConfigurationTests(ComponentTestApplicationFactory applicationFactory)
        {
            if (applicationFactory == null)
            {
                throw new ArgumentNullException(nameof(applicationFactory));
            }

            this.client = applicationFactory.CreateClient();
        }

        public void Dispose()
        {
            this.client.Dispose();
        }

        [Fact]
        public async Task GetConfiguration_ShouldReturn200Ok()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = "api/Configurations/1",
            };

            var response = await this.client.GetAsync(uriBuilder.Uri);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("global")]
        public async Task PutConfiguration_WithValidConfiguration_ShouldReturn200Ok(string id)
        {
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
            var response = await this.client.PutAsync(uriBuilder.Uri, contents);

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
            var response = await this.client.PutAsync(uriBuilder.Uri, contents);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
