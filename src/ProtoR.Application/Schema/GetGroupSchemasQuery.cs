namespace ProtoR.Application.Schema
{
    using System.Collections.Generic;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using ProtoR.Application.Common;

    public class GetGroupSchemasQuery : IRequest<PagedResult<SchemaDto>>
    {
        public string Name { get; set; }

        [FromQuery]
        public Pagination Pagination { get; set; } = Pagination.Default();

        [FromQuery]
        public IEnumerable<Filter> Filter { get; set; }

        [FromQuery]
        public IEnumerable<SortOrder> OrderBy { get; set; } = SortOrder.Default("Id");
    }
}
