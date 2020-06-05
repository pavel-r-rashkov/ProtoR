namespace ProtoR.ComponentTests
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using ProtoR.ComponentTests.Configuration;
    using ProtoR.Web.Resources.ClientResource;
    using Xunit;

    public class ClientTests : TestBase
    {
        public ClientTests(
            ComponentTestApplicationFactory applicationFactory,
            TestTokenProvider tokenProvider)
            : base(applicationFactory, tokenProvider)
        {
        }

        [Fact]
        public async Task GetClients_ShouldReturn200Ok()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = "api/Clients",
            };

            var response = await this.Client.GetAsync(uriBuilder.Uri);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task PostClient_ShouldReturn201Created()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = "api/Clients",
            };
            var authClient = new ClientWriteModel
            {
                ClientId = "new-client-id",
                ClientName = "new-client-name",
                Secret = "Qwertyuiopasdf1!",
                GrantTypes = new string[] { "client_credentials" },
                RoleBindings = new long[] { this.ApplicationFactory.RoleId },
                CategoryBindings = new long[] { this.ApplicationFactory.NonDefaultCategoryId },
            };

            using var contents = new JsonHttpContent(authClient);
            var response = await this.Client.PostAsync(uriBuilder.Uri, contents);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task PostClient_WithInvalidData_ShouldReturn400BadRequest()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = "api/Clients",
            };
            var authClient = new ClientWriteModel
            {
                ClientId = string.Empty,
                ClientName = "new-client-name",
                Secret = "Abc1!",
                GrantTypes = new string[] { "unknown_grant_type" },
                RoleBindings = new long[] { this.ApplicationFactory.RoleId },
                CategoryBindings = new long[] { this.ApplicationFactory.NonDefaultCategoryId },
            };

            using var contents = new JsonHttpContent(authClient);
            var response = await this.Client.PostAsync(uriBuilder.Uri, contents);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutClient_ShouldReturn204NoContent()
        {
            var id = this.ApplicationFactory.ClientId;
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Clients/{id}",
            };
            var authClient = new ClientWriteModel
            {
                ClientId = "client-id",
                ClientName = "client-name",
                Secret = "Qwertyuiopasdf1!",
                GrantTypes = new string[] { "client_credentials" },
                RoleBindings = new long[] { this.ApplicationFactory.RoleId },
                CategoryBindings = new long[] { this.ApplicationFactory.NonDefaultCategoryId },
            };

            using var contents = new JsonHttpContent(authClient);
            var response = await this.Client.PutAsync(uriBuilder.Uri, contents);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task PutClient_WithInvalidData_ShouldReturn400BadRequest()
        {
            var id = this.ApplicationFactory.ClientId;
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Clients/{id}",
            };
            var authClient = new ClientWriteModel
            {
                ClientId = "client-id",
                ClientName = string.Empty,
                Secret = "Abc",
                GrantTypes = new string[] { "invalid grant type" },
            };

            using var contents = new JsonHttpContent(authClient);
            var response = await this.Client.PutAsync(uriBuilder.Uri, contents);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteClient_ShouldReturn201NoContent()
        {
            var id = this.ApplicationFactory.ClientId;
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Clients/{id}",
            };

            var response = await this.Client.DeleteAsync(uriBuilder.Uri);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
