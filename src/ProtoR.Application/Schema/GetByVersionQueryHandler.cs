namespace ProtoR.Application.Schema
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class GetByVersionQueryHandler : IRequestHandler<GetByVersionQuery, SchemaDto>
    {
        public Task<SchemaDto> Handle(GetByVersionQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new SchemaDto());
        }
    }
}
