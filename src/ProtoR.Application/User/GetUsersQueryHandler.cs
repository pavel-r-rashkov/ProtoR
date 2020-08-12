namespace ProtoR.Application.User
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Application.Common;

    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
    {
        private readonly IUserDataProvider userDataProvider;

        public GetUsersQueryHandler(IUserDataProvider userDataProvider)
        {
            this.userDataProvider = userDataProvider;
        }

        public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            return await this.userDataProvider.GetUsers(
                request.Filter,
                request.OrderBy,
                request.Pagination);
        }
    }
}
