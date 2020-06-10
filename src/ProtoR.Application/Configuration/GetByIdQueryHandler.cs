namespace ProtoR.Application.Configuration
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Application.Group;
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.Exceptions;

    public class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, ConfigurationDto>
    {
        private readonly IConfigurationDataProvider configurationData;
        private readonly IGroupDataProvider groupData;
        private readonly IUserProvider userProvider;

        public GetByIdQueryHandler(
            IConfigurationDataProvider configurationData,
            IGroupDataProvider groupData,
            IUserProvider userProvider)
        {
            this.configurationData = configurationData;
            this.groupData = groupData;
            this.userProvider = userProvider;
        }

        public async Task<ConfigurationDto> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            ConfigurationDto configuration = request.ConfigurationId.Equals("global", StringComparison.InvariantCultureIgnoreCase)
                ? await this.configurationData.GetGlobalConfig()
                : await this.configurationData.GetById(Convert.ToInt64(request.ConfigurationId, CultureInfo.InvariantCulture));

            if (configuration == null)
            {
                throw new EntityNotFoundException<Configuration>((object)request.ConfigurationId);
            }

            var categories = this.userProvider.GetCategoryRestrictions();

            if (categories != null && configuration.GroupId != null)
            {
                var categoryId = await this.groupData.GetCategoryId(configuration.GroupId.Value);

                if (!categories.Contains(categoryId))
                {
                    throw new InaccessibleCategoryException(
                        categoryId,
                        this.userProvider.GetCurrentUserName());
                }
            }

            return configuration;
        }
    }
}
