namespace ProtoR.DataAccess.IntegrationTests.Fixtures
{
    using ProtoR.Infrastructure;

    public class UserProviderStub : IUserProvider
    {
        public string GetCurrentUserName()
        {
            return "Integration test user";
        }
    }
}
