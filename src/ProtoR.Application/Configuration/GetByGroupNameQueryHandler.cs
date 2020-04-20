namespace ProtoR.Application.Configuration
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class GetByGroupNameQueryHandler : IRequestHandler<GetByGroupNameQuery, ConfigurationDto>
    {
        private readonly IConfigurationDataProvider dataProvider;

        public GetByGroupNameQueryHandler(IConfigurationDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
        }

        public async Task<ConfigurationDto> Handle(GetByGroupNameQuery request, CancellationToken cancellationToken)
        {
            return await this.dataProvider.GetConfigByGroupName(request.GroupName);
        }
    }
}
