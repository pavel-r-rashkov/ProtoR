namespace ProtoR.Application.User
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IEnumerable<UserDto>>
    {
        private readonly IUserDataProvider userDataProvider;

        public GetUsersQueryHandler(IUserDataProvider userDataProvider)
        {
            this.userDataProvider = userDataProvider;
        }

        public async Task<IEnumerable<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            return await this.userDataProvider.GetUsers();
        }
    }
}
