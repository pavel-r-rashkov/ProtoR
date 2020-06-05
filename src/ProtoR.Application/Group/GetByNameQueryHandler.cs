namespace ProtoR.Application.Group
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Domain.CategoryAggregate;

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
            var group = await this.dataProvider.GetByName(request.GroupName);
            var categories = this.userProvider.GetCategoryRestrictions();

            if (categories != null && !categories.Contains(group.CategoryId))
            {
                throw new InaccessibleCategoryException(
                    group.CategoryId,
                    this.userProvider.GetCurrentUserName());
            }

            return group;
        }
    }
}
