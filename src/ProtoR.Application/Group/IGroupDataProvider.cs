namespace ProtoR.Application.Group
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IGroupDataProvider
    {
        Task<GroupDto> GetByName(string groupName);

        Task<string> GetGroupNameById(long id);

        Task<IEnumerable<GroupDto>> GetGroups(Expression<Func<GroupDto, bool>> filter);
    }
}
