namespace ProtoR.Application.Group
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Application.Common;
    using ProtoR.Domain.Exceptions;
    using ProtoR.Domain.SchemaGroupAggregate;

    public class GetByNameQueryHandler : IRequestHandler<GetByNameQuery, GroupDto>
    {
        private readonly IGroupDataProvider dataProvider;
        private readonly IUserProvider userProvider;

        public GetByNameQueryHandler(
            IGroupDataProvider dataProvider,
            IUserProvider userProvider)
        {
            this.dataProvider = dataProvider;
            this.userProvider = userProvider;
        }

        public async Task<GroupDto> Handle(GetByNameQuery request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            var group = await this.dataProvider.GetByName(request.Name);

            if (group == null)
            {
                throw new EntityNotFoundException<ProtoBufSchemaGroup>((object)request.Name);
            }

            var groupRestrictions = this.userProvider.GetGroupRestrictions();

            if (groupRestrictions != null)
            {
                var regex = FilterGenerator.CreateFromPatterns(groupRestrictions);
                var hasAccessToGroup = Regex.IsMatch(request.Name, regex, RegexOptions.IgnoreCase);

                if (!hasAccessToGroup)
                {
                    throw new InaccessibleGroupException(
                        request.Name,
                        this.userProvider.GetCurrentUserName());
                }
            }

            return group;
        }
    }
}
