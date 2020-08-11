namespace ProtoR.Application.Schema
{
    using System.Collections.Generic;
    using MediatR;

    public class GetGroupSchemasQuery : IRequest<IEnumerable<SchemaDto>>
    {
        public string Name { get; set; }
    }
}
