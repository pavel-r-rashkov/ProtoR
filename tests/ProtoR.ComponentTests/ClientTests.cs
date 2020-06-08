namespace ProtoR.ComponentTests
{
    using System;
    using System.Collections.Generic;
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
        public async Task GetClient_ShouldReturn200Ok()
        {
            var id = this.ApplicationFactory.ClientId;
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Clients/{id}",
            };

            var response = await this.Client.GetAsync(uriBuilder.Uri);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetClient_WithNonExistingClient_ShouldReturn404NotFound()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Clients/123456",
            };

            var response = await this.Client.GetAsync(uriBuilder.Uri);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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

        [Theory]
        [MemberData(nameof(InvalidClients))]
        public async Task PostClient_WithInvalidData_ShouldReturn400BadRequest(ClientWriteModel authClient)
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = "api/Clients",
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
        public async Task PutClient_WithNonExistingClient_ShouldReturn404NotFound()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = "api/Clients/123456",
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

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(InvalidClients))]
        public async Task PutClient_WithInvalidData_ShouldReturn400BadRequest(ClientWriteModel authClient)
        {
            var id = this.ApplicationFactory.ClientId;
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Clients/{id}",
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

        [Fact]
        public async Task DeleteClient_WithNonExistingClient_ShouldReturn404NotFound()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Clients/123456",
            };

            var response = await this.Client.DeleteAsync(uriBuilder.Uri);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        public static IEnumerable<object[]> InvalidClients()
        {
            yield return new object[]
            {
                new ClientWriteModel
                {
                    ClientId = null,
                    ClientName = "new-client-name",
                    Secret = "Qwertyuiop1!",
                    GrantTypes = new string[] { "client_credentials" },
                },
            };

            yield return new object[]
            {
                new ClientWriteModel
                {
                    ClientId = "client-id",
                    ClientName = null,
                    Secret = "Qwertyuiop1!",
                    GrantTypes = new string[] { "client_credentials" },
                },
            };

            yield return new object[]
            {
                new ClientWriteModel
                {
                    ClientId = "client-id",
                    ClientName = "new-client-name",
                    Secret = "Qwertyuiop1!",
                    GrantTypes = new string[] { "unknown_grant_type" },
                },
            };

            yield return new object[]
            {
                new ClientWriteModel
                {
                    ClientId = "client-id",
                    ClientName = "new-client-name",
                    Secret = "Abc1!",
                    GrantTypes = new string[] { "client_credentials" },
                },
            };

            yield return new object[]
            {
                new ClientWriteModel
                {
                    ClientId = "client-id",
                    ClientName = "new-client-name",
                    Secret = "Qwertyuiop1!",
                    GrantTypes = new string[] { "authorization_code" },
                    RedirectUris = Array.Empty<string>(),
                    PostLogoutRedirectUris = new string[] { "http://test.com/logout-redirect/" },
                    AllowedCorsOrigins = new string[] { "http://test.com" },
                },
            };

            yield return new object[]
            {
                new ClientWriteModel
                {
                    ClientId = "client-id",
                    ClientName = "new-client-name",
                    Secret = "Qwertyuiop1!",
                    GrantTypes = new string[] { "authorization_code" },
                    RedirectUris = Array.Empty<string>(),
                    PostLogoutRedirectUris = new string[] { "http://test.com/logout-redirect/" },
                    AllowedCorsOrigins = new string[] { "http://test.com" },
                },
            };

            yield return new object[]
            {
                new ClientWriteModel
                {
                    ClientId = "client-id",
                    ClientName = "new-client-name",
                    Secret = "Qwertyuiop1!",
                    GrantTypes = new string[] { "authorization_code" },
                    RedirectUris = new string[] { "http://test.com/redirect/" },
                    PostLogoutRedirectUris = Array.Empty<string>(),
                    AllowedCorsOrigins = new string[] { "http://test.com" },
                },
            };

            yield return new object[]
            {
                new ClientWriteModel
                {
                    ClientId = "client-id",
                    ClientName = "new-client-name",
                    Secret = "Qwertyuiop1!",
                    GrantTypes = new string[] { "authorization_code" },
                    RedirectUris = new string[] { "http://test.com/redirect/" },
                    PostLogoutRedirectUris = new string[] { "http://test.com/logout-redirect/" },
                    AllowedCorsOrigins = Array.Empty<string>(),
                },
            };

            yield return new object[]
            {
                new ClientWriteModel
                {
                    ClientId = "client-id",
                    ClientName = "new-client-name",
                    Secret = "Qwertyuiop1!",
                    GrantTypes = new string[] { "authorization_code" },
                    RedirectUris = new string[] { "invalidurl/redirect/" },
                    PostLogoutRedirectUris = new string[] { "http://test.com/logout-redirect/" },
                    AllowedCorsOrigins = new string[] { "http://test.com" },
                },
            };
        }
    }
}
