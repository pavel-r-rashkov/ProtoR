namespace ProtoR.Application.Schema
{
    using MediatR;

    public class GetByVersionQuery : IRequest<SchemaDto>
    {
        public string Name { get; set; }

        public string Version { get; set; }
    }
}
