namespace ProtoR.Application.Role
{
    using MediatR;

    public class DeleteRoleCommand : IRequest
    {
        public long RoleId { get; set; }
    }
}
