namespace ProtoR.Application.Schema
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class GetGroupSchemasQueryHandler : IRequestHandler<GetGroupSchemasQuery, IEnumerable<SchemaDto>>
    {
        private readonly ISchemaDataProvider dataProvider;

        public GetGroupSchemasQueryHandler(ISchemaDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
        }

        public async Task<IEnumerable<SchemaDto>> Handle(GetGroupSchemasQuery request, CancellationToken cancellationToken)
        {
            return await this.dataProvider.GetGroupSchemas(request.GroupName);
        }
    }
}