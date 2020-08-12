namespace ProtoR.Application.User
{
    using System.Collections.Generic;
    using MediatR;
    using ProtoR.Application.Common;

    public class GetUsersQuery : IRequest<PagedResult<UserDto>>
    {
        public Pagination Pagination { get; set; } = Pagination.Default();

        public IEnumerable<Filter> Filter { get; set; }

        public IEnumerable<SortOrder> OrderBy { get; set; } = SortOrder.Default("Id");
    }
}
