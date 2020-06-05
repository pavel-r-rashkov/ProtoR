namespace ProtoR.Application.User
{
    using System.Collections.Generic;
    using MediatR;

    public class GetUsersQuery : IRequest<IEnumerable<UserDto>>
    {
    }
}
