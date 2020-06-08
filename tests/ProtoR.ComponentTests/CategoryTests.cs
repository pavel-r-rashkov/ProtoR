namespace ProtoR.ComponentTests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using ProtoR.ComponentTests.Configuration;
    using ProtoR.Web.Resources.CategoryResource;
    using Xunit;

    public class CategoryTests : TestBase
    {
        public CategoryTests(
            ComponentTestApplicationFactory applicationFactory,
            TestTokenProvider tokenProvider)
            : base(applicationFactory, tokenProvider)
        {
        }

        [Fact]
        public async Task GetCategories_ShouldReturn200Ok()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = "api/Categories",
            };

            var response = await this.Client.GetAsync(uriBuilder.Uri);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetDefaultCategory_ShouldReturn200Ok()
        {
            var id = "default";
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Categories/{id}",
            };

            var response = await this.Client.GetAsync(uriBuilder.Uri);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetCategory_ShouldReturn200Ok()
        {
            var id = this.ApplicationFactory.NonDefaultCategoryId;
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Categories/{id}",
            };

            var response = await this.Client.GetAsync(uriBuilder.Uri);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetCategory_WithNonExistingCategory_ShouldReturn404NotFound()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Categories/123456",
            };

            var response = await this.Client.GetAsync(uriBuilder.Uri);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PostCategory_ShouldReturn201Created()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Categories",
            };
            var category = new CategoryWriteModel
            {
                Name = "New category",
            };

            using var content = new JsonHttpContent(category);
            var response = await this.Client.PostAsync(uriBuilder.Uri, content);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Theory]
        [MemberData(nameof(InvalidCategories))]
        public async Task PostCategory_WithInvalidData_ShouldReturn400BadRequest(CategoryWriteModel category)
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Categories",
            };

            using var content = new JsonHttpContent(category);
            var response = await this.Client.PostAsync(uriBuilder.Uri, content);

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteCategory_ShouldReturn204NoContent()
        {
            var id = this.ApplicationFactory.NonDefaultCategoryId;
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Categories/{id}",
            };

            var response = await this.Client.DeleteAsync(uriBuilder.Uri);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task DeleteCategory_WithNonExistingCategory_ShouldReturn404NotFound()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Categories/123456",
            };

            var response = await this.Client.DeleteAsync(uriBuilder.Uri);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PutCategory_ShouldReturn204NoContent()
        {
            var id = this.ApplicationFactory.NonDefaultCategoryId;
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Categories/{id}",
            };
            var category = new CategoryWriteModel
            {
                Name = "Updated category",
            };

            using var content = new JsonHttpContent(category);
            var response = await this.Client.PutAsync(uriBuilder.Uri, content);

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task PutCategory_WithNonExistingCategory_ShouldReturn404NotFound()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Categories/123456",
            };
            var category = new CategoryWriteModel
            {
                Name = "Updated category",
            };

            using var content = new JsonHttpContent(category);
            var response = await this.Client.PutAsync(uriBuilder.Uri, content);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [MemberData(nameof(InvalidCategories))]
        public async Task PutCategory_WithInvalidData_ShouldReturn400BadRequest(CategoryWriteModel category)
        {
            var id = this.ApplicationFactory.NonDefaultCategoryId;
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = $"api/Categories/{id}",
            };

            using var content = new JsonHttpContent(category);
            var response = await this.Client.PutAsync(uriBuilder.Uri, content);

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        public static IEnumerable<object[]> InvalidCategories()
        {
            yield return new object[]
            {
                new CategoryWriteModel
                {
                    Name = string.Empty,
                },
            };

            yield return new object[]
            {
                new CategoryWriteModel
                {
                    Name = null,
                },
            };

            yield return new object[]
            {
                new CategoryWriteModel
                {
                    Name = new string('a', 600),
                },
            };
        }
    }
}
