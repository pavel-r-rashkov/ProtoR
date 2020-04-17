namespace ProtoR.Application.Configuration
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, ConfigurationDto>
    {
        public Task<ConfigurationDto> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ConfigurationDto());
        }
    }
}
