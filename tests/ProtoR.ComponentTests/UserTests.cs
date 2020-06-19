namespace ProtoR.ComponentTests
{
    using System;
    using System.Collections.Generic;
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
        public async Task GetUser_ShouldReturn200Ok()
        {
            var id = this.ApplicationFactory.UserId;
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Users/{id}",
            };

            var response = await this.Client.GetAsync(uriBuilder.Uri);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetUser_WithNonExistingUser_ShouldReturn404NotFound()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Users/123456",
            };

            var response = await this.Client.GetAsync(uriBuilder.Uri);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
                Password = "Qwertyuiopas1!",
                GroupRestrictions = new string[] { "*" },
            };

            using var contents = new JsonHttpContent(user);
            var response = await this.Client.PostAsync(uriBuilder.Uri, contents);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(InvalidNewUsers))]
        public async Task PostUser_WithInvalidData_ShouldReturn400BadRequest(UserPostModel user)
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = "api/Users",
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
                GroupRestrictions = new string[] { "B*", "A*" },
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

        public static IEnumerable<object[]> InvalidNewUsers()
        {
            yield return new object[]
            {
                new UserPostModel
                {
                    UserName = null,
                    Password = "Qwertyuiopas1!",
                    GroupRestrictions = new string[] { "*" },
                },
            };

            yield return new object[]
            {
                new UserPostModel
                {
                    UserName = string.Empty,
                    Password = "Qwertyuiopas1!",
                    GroupRestrictions = new string[] { "*" },
                },
            };

            yield return new object[]
            {
                new UserPostModel
                {
                    UserName = "A-*bc",
                    Password = "Qwertyuiopas1!",
                    GroupRestrictions = new string[] { "*" },
                },
            };

            yield return new object[]
            {
                new UserPostModel
                {
                    UserName = "NewUser",
                    Password = null,
                    GroupRestrictions = new string[] { "*" },
                },
            };

            yield return new object[]
            {
                new UserPostModel
                {
                    UserName = "NewUser",
                    Password = "Abc1!",
                    GroupRestrictions = new string[] { "*" },
                },
            };

            yield return new object[]
            {
                new UserPostModel
                {
                    UserName = "NewUser",
                    Password = "qwertyuiopas1!",
                    GroupRestrictions = new string[] { "*" },
                },
            };

            yield return new object[]
            {
                new UserPostModel
                {
                    UserName = "NewUser",
                    Password = "QWERTYUIOPAS1!",
                    GroupRestrictions = new string[] { "*" },
                },
            };

            yield return new object[]
            {
                new UserPostModel
                {
                    UserName = "NewUser",
                    Password = "Qwertyuiopasd!",
                    GroupRestrictions = new string[] { "*" },
                },
            };

            yield return new object[]
            {
                new UserPostModel
                {
                    UserName = "NewUser",
                    Password = "Qwertyuiopas12",
                    GroupRestrictions = new string[] { "*" },
                },
            };

            yield return new object[]
            {
                new UserPostModel
                {
                    UserName = "NewUser",
                    Password = "Qwertyuiopas1!",
                    GroupRestrictions = Array.Empty<string>(),
                },
            };

            yield return new object[]
            {
                new UserPostModel
                {
                    UserName = "NewUser",
                    Password = "Qwertyuiopas1!",
                    GroupRestrictions = new string[] { "Abc&*" },
                },
            };
        }
    }
}
