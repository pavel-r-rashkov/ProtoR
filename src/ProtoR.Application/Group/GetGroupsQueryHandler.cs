namespace ProtoR.Application.Group
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class GetGroupsQueryHandler : IRequestHandler<GetGroupsQuery, IEnumerable<GroupDto>>
    {
        private readonly IGroupDataProvider dataProvider;

        public GetGroupsQueryHandler(IGroupDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
        }

        public async Task<IEnumerable<GroupDto>> Handle(GetGroupsQuery request, CancellationToken cancellationToken)
        {
            return await this.dataProvider.GetGroups();
        }
    }
}
