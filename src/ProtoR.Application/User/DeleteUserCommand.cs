namespace ProtoR.Application.User
{
    using MediatR;

    public class DeleteUserCommand : IRequest
    {
        public long UserId { get; set; }
    }
}
