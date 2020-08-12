namespace ProtoR.Application.Group
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using ProtoR.Application.Common;

    public interface IGroupDataProvider
    {
        Task<GroupDto> GetByName(string groupName);

        Task<string> GetGroupNameById(long id);

        Task<PagedResult<GroupDto>> GetGroups(
            Expression<Func<GroupDto, bool>> filter,
            IEnumerable<Filter> filters,
            IEnumerable<SortOrder> sortOrders,
            Pagination pagination);
    }
}
