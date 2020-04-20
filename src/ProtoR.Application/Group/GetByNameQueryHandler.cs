namespace ProtoR.Application.Group
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class GetByNameQueryHandler : IRequestHandler<GetByNameQuery, GroupDto>
    {
        private readonly IGroupDataProvider dataProvider;

        public GetByNameQueryHandler(IGroupDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
        }

        public async Task<GroupDto> Handle(GetByNameQuery request, CancellationToken cancellationToken)
        {
            return await this.dataProvider.GetByName(request.GroupName);
        }
    }
}
