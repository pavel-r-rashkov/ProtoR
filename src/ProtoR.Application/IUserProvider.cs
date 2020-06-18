namespace ProtoR.Application
{
    using System.Collections.Generic;

    public interface IUserProvider
    {
        string GetCurrentUserName();

        IEnumerable<string> GetGroupRestrictions();
    }
}
