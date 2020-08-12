namespace ProtoR.Application.Client
{
    using System.Collections.Generic;
    using MediatR;
    using ProtoR.Application.Common;

    public class GetClientsQuery : IRequest<PagedResult<ClientDto>>
    {
        public Pagination Pagination { get; set; } = Pagination.Default();

        public IEnumerable<Filter> Filter { get; set; }

        public IEnumerable<SortOrder> OrderBy { get; set; } = SortOrder.Default("Id");
    }
}
