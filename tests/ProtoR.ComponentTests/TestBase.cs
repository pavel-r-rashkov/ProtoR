namespace ProtoR.ComponentTests
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using IdentityModel.Client;
    using ProtoR.ComponentTests.Configuration;
    using Xunit;

    [Collection(CollectionNames.TestApplicationCollection)]
    public class TestBase : IDisposable, IAsyncLifetime
    {
        private readonly TestTokenProvider tokenProvider;
        private bool disposed = false;

        public TestBase(
            ComponentTestApplicationFactory applicationFactory,
            TestTokenProvider tokenProvider)
        {
            this.ApplicationFactory = applicationFactory ?? throw new ArgumentNullException(nameof(applicationFactory));
            this.Client = applicationFactory.CreateClient();
            this.tokenProvider = tokenProvider;
        }

        protected HttpClient Client { get; private set; }

        protected ComponentTestApplicationFactory ApplicationFactory { get; private set; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task InitializeAsync()
        {
            await this.ApplicationFactory.Seed();
            var token = await this.tokenProvider.RequestAccessToken(this.Client);
            this.Client.SetBearerToken(token);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.ApplicationFactory.Clear();
                this.Client.Dispose();
                this.tokenProvider.Clear();
            }

            this.disposed = true;
        }
    }
}
