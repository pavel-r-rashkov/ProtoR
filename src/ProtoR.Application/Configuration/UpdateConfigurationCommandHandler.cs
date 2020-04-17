namespace ProtoR.Application.Configuration
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class UpdateConfigurationCommandHandler : AsyncRequestHandler<UpdateConfigurationCommand>
    {
        protected override Task Handle(UpdateConfigurationCommand request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
