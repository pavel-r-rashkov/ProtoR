namespace ProtoR.Application.Configuration
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Application.Common;
    using ProtoR.Application.Group;
    using ProtoR.Domain.Exceptions;
    using ProtoR.Domain.SchemaGroupAggregate;

    public class GetByGroupNameQueryHandler : IRequestHandler<GetByGroupNameQuery, ConfigurationDto>
    {
        private readonly IConfigurationDataProvider dataProvider;
        private readonly IUserProvider userProvider;

        public GetByGroupNameQueryHandler(
            IConfigurationDataProvider dataProvider,
            IUserProvider userProvider)
        {
            this.dataProvider = dataProvider;
            this.userProvider = userProvider;
        }

        public async Task<ConfigurationDto> Handle(GetByGroupNameQuery request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            var configuration = await this.dataProvider.GetConfigByGroupName(request.GroupName);

            if (configuration == null)
            {
                throw new EntityNotFoundException<ProtoBufSchemaGroup>((object)request.GroupName);
            }

            var groupRestrictions = this.userProvider.GetGroupRestrictions();

            if (groupRestrictions != null && configuration.GroupId != null)
            {
                var regex = FilterGenerator.CreateFromPatterns(groupRestrictions);
                var hasAccessToGroup = Regex.IsMatch(request.GroupName, regex, RegexOptions.IgnoreCase);

                if (!hasAccessToGroup)
                {
                    throw new InaccessibleGroupException(
                        request.GroupName,
                        this.userProvider.GetCurrentUserName());
                }
            }

            return configuration;
        }
    }
}
