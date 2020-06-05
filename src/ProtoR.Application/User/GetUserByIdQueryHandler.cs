namespace ProtoR.Application.User
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUserDataProvider userDataProvider;

        public GetUserByIdQueryHandler(IUserDataProvider userDataProvider)
        {
            this.userDataProvider = userDataProvider;
        }

        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            return await this.userDataProvider.GetById(request.UserId);
        }
    }
}
