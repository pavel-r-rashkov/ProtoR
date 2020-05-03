namespace ProtoR.ComponentTests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Moq;
    using ProtoR.ComponentTests.Configuration;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Web.Resources.GroupResource;
    using ProtoR.Web.Resources.SchemaResource;
    using Xunit;

    [Collection(CollectionNames.TestApplicationCollection)]
    public sealed class GroupTests : IDisposable
    {
        private readonly HttpClient client;
        private readonly ComponentTestApplicationFactory applicationFactory;

        public GroupTests(ComponentTestApplicationFactory applicationFactory)
        {
            this.applicationFactory = applicationFactory ?? throw new ArgumentNullException(nameof(applicationFactory));
            this.client = applicationFactory.CreateClient();
        }

        public void Dispose()
        {
            this.applicationFactory.ResetMockSetup();
            this.client.Dispose();
        }

        [Fact]
        public async Task GetGroups_ShouldReturn200Ok()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = "api/Groups",
            };

            var response = await this.client.GetAsync(uriBuilder.Uri);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetGroup_ShouldReturn200Ok()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups/TestGroupName",
            };

            var response = await this.client.GetAsync(uriBuilder.Uri);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetGroupSchemas_ShouldReturn200Ok()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups/TestGroupName/Schemas",
            };

            var response = await this.client.GetAsync(uriBuilder.Uri);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Theory]
        [InlineData("2")]
        [InlineData("latest")]
        public async Task GetGroupSchemaByVersion_ShouldReturn200Ok(string version)
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups/TestGroupName/Schemas/{version}",
            };

            var response = await this.client.GetAsync(uriBuilder.Uri);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetGroupConfiguration_ShouldReturn200Ok()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups/TestGroupName/Configuration",
            };

            var response = await this.client.GetAsync(uriBuilder.Uri);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task CreateGroup_ShouldReturn200Ok()
        {
            this.applicationFactory.GroupRepositoryMock
                .Setup(r => r.GetByName(It.IsAny<string>()))
                .ReturnsAsync((ProtoBufSchemaGroup)null);

            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups",
            };
            var group = new GroupWriteModel
            {
                Name = "New group name",
            };

            using var contents = new JsonHttpContent(group);
            var response = await this.client.PostAsync(uriBuilder.Uri, contents);

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
                Name = "New group name",
            };

            using var contents = new JsonHttpContent(group);
            var response = await this.client.PostAsync(uriBuilder.Uri, contents);

            Assert.False(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task CreateSchema_WithValidSchema_ShouldReturn200Ok()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups/TestGroup/Schemas",
            };
            var group = new SchemaWriteModel
            {
                Contents = "syntax = \"proto3\";",
            };

            using var contents = new JsonHttpContent(group);
            var response = await this.client.PostAsync(uriBuilder.Uri, contents);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task CreateSchema_WithInvalidSchema_ShouldReturn400BadRequest()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups/TestGroup/Schemas",
            };
            var group = new SchemaWriteModel
            {
                Contents = "Invalid schema",
            };

            using var contents = new JsonHttpContent(group);
            var response = await this.client.PostAsync(uriBuilder.Uri, contents);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task SchemaTest_WithValidSchema_ShouldReturn200Ok()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups/TestGroup/SchemaTest",
            };
            var group = new SchemaWriteModel
            {
                Contents = "syntax = \"proto3\";",
            };

            using var contents = new JsonHttpContent(group);
            var response = await this.client.PostAsync(uriBuilder.Uri, contents);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task SchemaTest_WithInvalidSchema_ShouldReturn400BadRequest()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups/TestGroup/SchemaTest",
            };
            var group = new SchemaWriteModel
            {
                Contents = "Invalid schema",
            };

            using var contents = new JsonHttpContent(group);
            var response = await this.client.PostAsync(uriBuilder.Uri, contents);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteGroup_ShouldReturnNoContent()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Groups/TestGroup",
            };

            var response = await this.client.DeleteAsync(uriBuilder.Uri);

            Assert.True(response.IsSuccessStatusCode);
        }
    }
}
