namespace ProtoR.Web.Infrastructure
{
    using ProtoR.Infrastructure;

    public class UserProvider : IUserProvider
    {
        public string GetCurrentUserName()
        {
            return "Test User";
        }
    }
}
