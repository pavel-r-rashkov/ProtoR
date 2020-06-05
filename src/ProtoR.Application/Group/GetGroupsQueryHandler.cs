namespace ProtoR.Application.Group
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

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
            var categories = this.userProvider.GetCategoryRestrictions();

            return await this.dataProvider.GetGroups(categories);
        }
    }
}
