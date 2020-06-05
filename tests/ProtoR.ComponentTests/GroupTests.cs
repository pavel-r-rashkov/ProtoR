namespace ProtoR.ComponentTests
{
    using System;
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
                GroupName = "New group name",
                CategoryId = this.ApplicationFactory.NonDefaultCategoryId,
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
                GroupName = "Test Group",
            };

            using var contents = new JsonHttpContent(group);
            var response = await this.Client.PostAsync(uriBuilder.Uri, contents);

            Assert.False(response.IsSuccessStatusCode);
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
    }
}
