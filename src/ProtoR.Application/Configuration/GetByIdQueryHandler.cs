namespace ProtoR.Application.Configuration
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, ConfigurationDto>
    {
        private readonly IConfigurationDataProvider dataProvider;

        public GetByIdQueryHandler(IConfigurationDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
        }

        public async Task<ConfigurationDto> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        {
            ConfigurationDto configuration = request.ConfigurationId.Equals("global", StringComparison.InvariantCultureIgnoreCase)
                ? await this.dataProvider.GetGlobalConfig()
                : await this.dataProvider.GetById(Convert.ToInt64(request.ConfigurationId, CultureInfo.InvariantCulture));

            return configuration;
        }
    }
}
