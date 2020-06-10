namespace ProtoR.Application.User
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Domain.Exceptions;
    using ProtoR.Domain.UserAggregate;

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUserDataProvider userDataProvider;

        public GetUserByIdQueryHandler(IUserDataProvider userDataProvider)
        {
            this.userDataProvider = userDataProvider;
        }

        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            var user = await this.userDataProvider.GetById(request.UserId);

            if (user == null)
            {
                throw new EntityNotFoundException<User>(request.UserId);
            }

            return user;
        }
    }
}
