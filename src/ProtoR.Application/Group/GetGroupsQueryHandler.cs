namespace ProtoR.Application.Group
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Application.Common;

    public class GetGroupsQueryHandler : IRequestHandler<GetGroupsQuery, IEnumerable<GroupDto>>
    {
        private readonly IGroupDataProvider dataProvider;
        private readonly IUserProvider userProvider;

        public GetGroupsQueryHandler(
            IGroupDataProvider dataProvider,
            IUserProvider userProvider)
        {
            this.dataProvider = dataProvider;
            this.userProvider = userProvider;
        }

        public async Task<IEnumerable<GroupDto>> Handle(GetGroupsQuery request, CancellationToken cancellationToken)
        {
            var groupRestrictions = this.userProvider.GetGroupRestrictions();
            Expression<Func<GroupDto, bool>> filter = null;

            if (groupRestrictions != null)
            {
                var regex = FilterGenerator.CreateFromPatterns(groupRestrictions);
                filter = f => Regex.IsMatch(f.Name, regex, RegexOptions.IgnoreCase);
            }

            return await this.dataProvider.GetGroups(filter);
        }
    }
}
