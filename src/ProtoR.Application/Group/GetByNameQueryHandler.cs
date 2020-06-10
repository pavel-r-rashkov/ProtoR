namespace ProtoR.Application.Group
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
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
            var group = await this.dataProvider.GetByName(request.GroupName);

            if (group == null)
            {
                throw new EntityNotFoundException<ProtoBufSchemaGroup>((object)request.GroupName);
            }

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
