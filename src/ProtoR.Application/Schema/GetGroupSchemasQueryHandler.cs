namespace ProtoR.Application.Schema
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class GetGroupSchemasQueryHandler : IRequestHandler<GetGroupSchemasQuery, IEnumerable<SchemaDto>>
    {
        public Task<IEnumerable<SchemaDto>> Handle(GetGroupSchemasQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new List<SchemaDto> { new SchemaDto() }.AsEnumerable());
        }
    }
}
