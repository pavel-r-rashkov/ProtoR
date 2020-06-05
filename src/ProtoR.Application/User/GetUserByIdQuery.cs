namespace ProtoR.Application.User
{
    using MediatR;

    public class GetUserByIdQuery : IRequest<UserDto>
    {
        public long UserId { get; set; }
    }
}
