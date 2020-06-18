namespace ProtoR.DataAccess.IntegrationTests.Fixtures
{
    using System.Collections.Generic;
    using ProtoR.Application;

    public class UserProviderStub : IUserProvider
    {
        public IEnumerable<string> GetGroupRestrictions()
        {
            return null;
        }

        public string GetCurrentUserName()
        {
            return "Integration test user";
        }
    }
}
