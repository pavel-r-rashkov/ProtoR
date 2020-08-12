namespace ProtoR.Application.User
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ProtoR.Application.Common;

    public interface IUserDataProvider
    {
         Task<UserDto> GetById(long id);

         Task<PagedResult<UserDto>> GetUsers(
             IEnumerable<Filter> filters,
             IEnumerable<SortOrder> sortOrders,
             Pagination pagination);
    }
}
