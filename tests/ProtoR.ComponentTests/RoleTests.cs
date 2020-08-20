namespace ProtoR.ComponentTests
{
    using System;
    using System.Collections.Generic;
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
        public async Task GetRole_ShouldReturn200Ok()
        {
            var id = this.ApplicationFactory.RoleId;
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Roles/{id}",
            };

            var response = await this.Client.GetAsync(uriBuilder.Uri);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetRole_WithNonExistingRole_ShouldReturn404NotFound()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Roles/123456",
            };

            var response = await this.Client.GetAsync(uriBuilder.Uri);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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

        [Theory]
        [MemberData(nameof(InvalidRoles))]
        public async Task PostRole_WithInvalidData_ShouldReturn422UnprocessableEntity(RoleWriteModel role)
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = "api/Roles",
            };

            using var contents = new JsonHttpContent(role);
            var response = await this.Client.PostAsync(uriBuilder.Uri, contents);

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
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

        [Theory]
        [MemberData(nameof(InvalidRoles))]
        public async Task PutRole_WithInvalidData_ShouldReturn422UnprocessableEntity(RoleWriteModel role)
        {
            var id = this.ApplicationFactory.RoleId;
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Roles/{id}",
            };

            using var contents = new JsonHttpContent(role);
            var response = await this.Client.PutAsync(uriBuilder.Uri, contents);

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
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

        public static IEnumerable<object[]> InvalidRoles()
        {
            yield return new object[]
            {
                new RoleWriteModel
                {
                    Name = null,
                    Permissions = new int[]
                    {
                        (int)Permission.UserWrite,
                        (int)Permission.SchemaWrite,
                    },
                },
            };

            yield return new object[]
            {
                new RoleWriteModel
                {
                    Name = string.Empty,
                    Permissions = new int[]
                    {
                        (int)Permission.UserWrite,
                        (int)Permission.SchemaWrite,
                    },
                },
            };

            yield return new object[]
            {
                new RoleWriteModel
                {
                    Name = new string('a', 501),
                    Permissions = new int[]
                    {
                        (int)Permission.UserWrite,
                        (int)Permission.SchemaWrite,
                    },
                },
            };
        }
    }
}
