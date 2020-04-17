namespace ProtoR.Application.Group
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class CreateGroupCommandHandler : AsyncRequestHandler<CreateGroupCommand>
    {
        protected override Task Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
