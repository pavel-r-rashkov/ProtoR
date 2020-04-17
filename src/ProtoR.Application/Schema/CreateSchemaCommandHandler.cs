namespace ProtoR.Application.Schema
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class CreateSchemaCommandHandler : IRequestHandler<CreateSchemaCommand, string>
    {
        public Task<string> Handle(CreateSchemaCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult("1");
        }
    }
}
