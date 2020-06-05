namespace ProtoR.ComponentTests
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using ProtoR.ComponentTests.Configuration;
    using ProtoR.Web.Infrastructure.Identity;
    using ProtoR.Web.Resources.RoleResource;
    using Xunit;

    public class RoleTests : TestBase
    {
        public RoleTests(
            ComponentTestApplicationFactory applicationFactory,
            TestTokenProvider tokenProvider)
            : base(applicationFactory, tokenProvider)
        {
        }

        [Fact]
        public async Task GetRoles_ShouldReturn200Ok()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = "api/Roles",
            };

            var response = await this.Client.GetAsync(uriBuilder.Uri);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task PostRole_ShouldReturn201Created()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = "api/Roles",
            };
            var role = new RoleWriteModel
            {
                Name = "Test Role",
                Permissions = new int[]
                {
                    (int)Permission.CategoryRead,
                    (int)Permission.RoleWrite,
                    (int)Permission.SchemaWrite,
                },
            };

            using var contents = new JsonHttpContent(role);
            var response = await this.Client.PostAsync(uriBuilder.Uri, contents);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task PostRole_WithInvalidData_ShouldReturn400BadRequest()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = "api/Roles",
            };
            var role = new RoleWriteModel
            {
                Name = new string('a', 600),
                Permissions = new int[]
                {
                    (int)Permission.CategoryRead,
                    (int)Permission.RoleWrite,
                    (int)Permission.SchemaWrite,
                },
            };

            using var contents = new JsonHttpContent(role);
            var response = await this.Client.PostAsync(uriBuilder.Uri, contents);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutRole_ShouldReturn204NoContent()
        {
            var id = this.ApplicationFactory.RoleId;
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Roles/{id}",
            };
            var role = new RoleWriteModel
            {
                Name = "Updated Name",
                Permissions = new int[]
                {
                    (int)Permission.UserWrite,
                    (int)Permission.SchemaWrite,
                },
            };

            using var contents = new JsonHttpContent(role);
            var response = await this.Client.PutAsync(uriBuilder.Uri, contents);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task PutRole_WithInvalidData_ShouldReturn400BadRequest()
        {
            var id = this.ApplicationFactory.RoleId;
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Roles/{id}",
            };
            var role = new RoleWriteModel
            {
                Name = new string('a', 600),
                Permissions = new int[]
                {
                    (int)Permission.UserWrite,
                    (int)Permission.SchemaWrite,
                },
            };

            using var contents = new JsonHttpContent(role);
            var response = await this.Client.PutAsync(uriBuilder.Uri, contents);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteRole_ShouldReturn201NoContent()
        {
            var id = this.ApplicationFactory.RoleId;
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Roles/{id}",
            };

            var response = await this.Client.DeleteAsync(uriBuilder.Uri);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
