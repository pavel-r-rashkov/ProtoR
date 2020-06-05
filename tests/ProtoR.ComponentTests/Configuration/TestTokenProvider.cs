namespace ProtoR.ComponentTests.Configuration
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using IdentityModel.Client;

    public class TestTokenProvider
    {
        private string token;

        public async Task<string> RequestAccessToken(HttpClient client)
        {
            if (this.token == null)
            {
                var discoveryDocument = await client.GetDiscoveryDocumentAsync(Constants.BaseAddress);

                using var tokenRequest = new ClientCredentialsTokenRequest
                {
                    Address = discoveryDocument.TokenEndpoint,
                    ClientId = "client ID",
                    ClientSecret = "testsecret",
                    Scope = "protor-api",
                };
                var tokenResponse = await client.RequestClientCredentialsTokenAsync(tokenRequest);
                this.token = tokenResponse.AccessToken;
            }

            return this.token;
        }

        public void Clear()
        {
            this.token = null;
        }
    }
}
