namespace ProtoR.Application.Role
{
    using System.Collections.Generic;
    using MediatR;
    using ProtoR.Application.Common;

    public class GetRolesQuery : IRequest<PagedResult<RoleDto>>
    {
        public Pagination Pagination { get; set; } = Pagination.Default();

        public IEnumerable<Filter> Filter { get; set; }

        public IEnumerable<SortOrder> OrderBy { get; set; } = SortOrder.Default("Id");
    }
}
