namespace ProtoR.Application.Role
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ProtoR.Application.Common;

    public interface IRoleDataProvider
    {
        Task<RoleDto> GetById(long id);

        Task<PagedResult<RoleDto>> GetRoles(
            IEnumerable<Filter> filters,
            IEnumerable<SortOrder> sortOrders,
            Pagination pagination);
    }
}
