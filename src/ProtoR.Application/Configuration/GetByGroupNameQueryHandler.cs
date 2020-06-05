namespace ProtoR.Application.Configuration
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Application.Group;
    using ProtoR.Domain.CategoryAggregate;

    public class GetByGroupNameQueryHandler : IRequestHandler<GetByGroupNameQuery, ConfigurationDto>
    {
        private readonly IConfigurationDataProvider dataProvider;
        private readonly IGroupDataProvider groupData;
        private readonly IUserProvider userProvider;

        public GetByGroupNameQueryHandler(
            IConfigurationDataProvider dataProvider,
            IGroupDataProvider groupData,
            IUserProvider userProvider)
        {
            this.dataProvider = dataProvider;
            this.groupData = groupData;
            this.userProvider = userProvider;
        }

        public async Task<ConfigurationDto> Handle(GetByGroupNameQuery request, CancellationToken cancellationToken)
        {
            var configuration = await this.dataProvider.GetConfigByGroupName(request.GroupName);
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
