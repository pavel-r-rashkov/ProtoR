namespace ProtoR.Application.Role
{
    using System.Collections.Generic;
    using MediatR;

    public class GetRolesQuery : IRequest<IEnumerable<RoleDto>>
    {
    }
}
