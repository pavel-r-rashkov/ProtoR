namespace ProtoR.Application.Group
{
    using System.Collections.Generic;
    using MediatR;
    using ProtoR.Application.Common;

    public class GetGroupsQuery : IRequest<PagedResult<GroupDto>>
    {
        public Pagination Pagination { get; set; } = Pagination.Default();

        public IEnumerable<Filter> Filter { get; set; }

        public IEnumerable<SortOrder> OrderBy { get; set; } = SortOrder.Default("Id");
    }
}
