namespace ProtoR.ComponentTests
{
    using System;
    using System.Threading.Tasks;
    using ProtoR.ComponentTests.Configuration;
    using Xunit;

    public class PermissionTests : TestBase
    {
        public PermissionTests(
            ComponentTestApplicationFactory applicationFactory,
            TestTokenProvider tokenProvider)
            : base(applicationFactory, tokenProvider)
        {
        }

        [Fact]
        public async Task GetPermissions_ShouldReturn200Ok()
        {
            var uriBuilder = new UriBuilder(Constants.BaseAddress)
            {
                Path = "api/Permissions",
            };

            var response = await this.Client.GetAsync(uriBuilder.Uri);
            Assert.True(response.IsSuccessStatusCode);
        }
    }
}
