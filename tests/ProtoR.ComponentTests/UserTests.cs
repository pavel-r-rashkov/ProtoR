namespace ProtoR.ComponentTests
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using ProtoR.ComponentTests.Configuration;
    using ProtoR.Web.Resources.UserResource;
    using Xunit;

    public class UserTests : TestBase
    {
        public UserTests(
            ComponentTestApplicationFactory applicationFactory,
            TestTokenProvider tokenProvider)
            : base(applicationFactory, tokenProvider)
        {
        }

        [Fact]
        public async Task GetUsers_ShouldReturn200Ok()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = "api/Users",
            };

            var response = await this.Client.GetAsync(uriBuilder.Uri);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostUser_ShouldReturn201Created()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = "api/Users",
            };
            var user = new UserPostModel
            {
                UserName = "NewTestUser",
                Password = "Qwertyuiop1!",
            };

            using var contents = new JsonHttpContent(user);
            var response = await this.Client.PostAsync(uriBuilder.Uri, contents);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task PostUser_WithInvalidData_ShouldReturn400BadRequest()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = "api/Users",
            };
            var user = new UserPostModel
            {
                UserName = "TestUser",
                Password = "A1!",
            };

            using var contents = new JsonHttpContent(user);
            var response = await this.Client.PostAsync(uriBuilder.Uri, contents);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PutUser_ShouldReturn204NoContent()
        {
            var id = this.ApplicationFactory.UserId;
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Users/{id}",
            };
            var user = new UserPutModel
            {
                Roles = new long[] { this.ApplicationFactory.RoleId },
                Categories = new long[] { this.ApplicationFactory.NonDefaultCategoryId },
            };

            using var contents = new JsonHttpContent(user);
            var response = await this.Client.PutAsync(uriBuilder.Uri, contents);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_ShouldReturn201NoContent()
        {
            var id = this.ApplicationFactory.UserId;
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Users/{id}",
            };

            var response = await this.Client.DeleteAsync(uriBuilder.Uri);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
