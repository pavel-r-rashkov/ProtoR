namespace ProtoR.Application.Schema
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Application.Common;

    public class GetGroupSchemasQueryHandler : IRequestHandler<GetGroupSchemasQuery, PagedResult<SchemaDto>>
    {
        private readonly ISchemaDataProvider dataProvider;

        public GetGroupSchemasQueryHandler(ISchemaDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
        }

        public async Task<PagedResult<SchemaDto>> Handle(GetGroupSchemasQuery request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            return await this.dataProvider.GetGroupSchemas(
                request.Name,
                request.Filter,
                request.OrderBy,
                request.Pagination);
        }
    }
}
