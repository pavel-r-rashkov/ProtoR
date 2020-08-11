namespace ProtoR.ComponentTests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using ProtoR.ComponentTests.Configuration;
    using ProtoR.Web.Resources.GroupResource;
    using ProtoR.Web.Resources.SchemaResource;
    using Xunit;

    public class GroupTests : TestBase
    {
        public GroupTests(
            ComponentTestApplicationFactory applicationFactory,
            TestTokenProvider tokenProvider)
            : base(applicationFactory, tokenProvider)
        {
        }

        [Fact]
        public async Task GetGroups_ShouldReturn200Ok()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = "api/Groups",
            };

            var response = await this.Client.GetAsync(uriBuilder.Uri);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetGroup_ShouldReturn200Ok()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups/Test Group",
            };

            var response = await this.Client.GetAsync(uriBuilder.Uri);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetGroup_WithNonExistingGroup_ShouldReturn404NotFound()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups/SomeNonExistingGroup",
            };

            var response = await this.Client.GetAsync(uriBuilder.Uri);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetGroupSchemas_ShouldReturn200Ok()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups/Test Group/Schemas",
            };

            var response = await this.Client.GetAsync(uriBuilder.Uri);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData("2")]
        [InlineData("latest")]
        public async Task GetGroupSchemaByVersion_ShouldReturn200Ok(string version)
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups/Test Group/Schemas/{version}",
            };

            var response = await this.Client.GetAsync(uriBuilder.Uri);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetGroupSchemaByVersion_WithNonExistingSchema_ShouldReturn404NotFound()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups/Test Group/Schemas/999",
            };

            var response = await this.Client.GetAsync(uriBuilder.Uri);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetGroupConfiguration_ShouldReturn200Ok()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups/Test Group/Configuration",
            };

            var response = await this.Client.GetAsync(uriBuilder.Uri);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task CreateGroup_ShouldReturn200Ok()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups",
            };
            var group = new GroupWriteModel
            {
                Name = "New group name",
            };

            using var contents = new JsonHttpContent(group);
            var response = await this.Client.PostAsync(uriBuilder.Uri, contents);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task CreateGroup_WithDuplicatedGroupName_ShouldReturn400BadRequest()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups",
            };
            var group = new GroupWriteModel
            {
                Name = "Test Group",
            };

            using var contents = new JsonHttpContent(group);
            var response = await this.Client.PostAsync(uriBuilder.Uri, contents);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(InvalidGroups))]
        public async Task CreateGroup_WithInvalidData_ShouldReturn400BadRequest(GroupWriteModel group)
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups",
            };

            using var contents = new JsonHttpContent(group);
            var response = await this.Client.PostAsync(uriBuilder.Uri, contents);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateSchema_WithValidSchema_ShouldReturn200Ok()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups/Test Group/Schemas",
            };
            var group = new SchemaWriteModel
            {
                Contents = "syntax = \"proto3\";",
            };

            using var contents = new JsonHttpContent(group);
            var response = await this.Client.PostAsync(uriBuilder.Uri, contents);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task CreateSchema_WithNonExistingGroup_ShouldReturn404NotFound()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups/NonExistingGroup/Schemas",
            };
            var group = new SchemaWriteModel
            {
                Contents = "syntax = \"proto3\";",
            };

            using var contents = new JsonHttpContent(group);
            var response = await this.Client.PostAsync(uriBuilder.Uri, contents);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateSchema_WithInvalidSchema_ShouldReturn400BadRequest()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups/Test Group/Schemas",
            };
            var group = new SchemaWriteModel
            {
                Contents = "Invalid schema",
            };

            using var contents = new JsonHttpContent(group);
            var response = await this.Client.PostAsync(uriBuilder.Uri, contents);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task SchemaTest_WithValidSchema_ShouldReturn200Ok()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups/Test Group/SchemaTest",
            };
            var group = new SchemaWriteModel
            {
                Contents = "syntax = \"proto3\";",
            };

            using var contents = new JsonHttpContent(group);
            var response = await this.Client.PostAsync(uriBuilder.Uri, contents);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task SchemaTest_WithNonExistingGroup_ShouldReturn404NotFound()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups/NonExistingGroup/SchemaTest",
            };
            var group = new SchemaWriteModel
            {
                Contents = "syntax = \"proto3\";",
            };

            using var contents = new JsonHttpContent(group);
            var response = await this.Client.PostAsync(uriBuilder.Uri, contents);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task SchemaTest_WithInvalidSchema_ShouldReturn400BadRequest()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups/Test Group/SchemaTest",
            };
            var group = new SchemaWriteModel
            {
                Contents = "Invalid schema",
            };

            using var contents = new JsonHttpContent(group);
            var response = await this.Client.PostAsync(uriBuilder.Uri, contents);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteGroup_ShouldReturnNoContent()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups/Test Group",
            };

            var response = await this.Client.DeleteAsync(uriBuilder.Uri);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task DeleteGroup_WithNonExistingGroup_ShouldReturn404NotFound()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups/NonExistingGroup",
            };

            var response = await this.Client.DeleteAsync(uriBuilder.Uri);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        public static IEnumerable<object[]> InvalidGroups()
        {
            yield return new object[]
            {
                new GroupWriteModel
                {
                    Name = null,
                },
            };

            yield return new object[]
            {
                new GroupWriteModel
                {
                    Name = string.Empty,
                },
            };

            yield return new object[]
            {
                new GroupWriteModel
                {
                    Name = new string('a', 501),
                },
            };
        }
    }
}
