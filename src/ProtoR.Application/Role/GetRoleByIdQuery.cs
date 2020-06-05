namespace ProtoR.Application.Role
{
    using MediatR;

    public class GetRoleByIdQuery : IRequest<RoleDto>
    {
        public long RoleId { get; set; }
    }
}
