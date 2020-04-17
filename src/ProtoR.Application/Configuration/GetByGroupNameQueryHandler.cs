namespace ProtoR.Application.Configuration
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class GetByGroupNameQueryHandler : IRequestHandler<GetByGroupNameQuery, ConfigurationDto>
    {
        public Task<ConfigurationDto> Handle(GetByGroupNameQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ConfigurationDto());
        }
    }
}
