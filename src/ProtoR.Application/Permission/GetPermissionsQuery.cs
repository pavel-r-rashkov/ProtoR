namespace ProtoR.Application.Permission
{
    using System.Collections.Generic;
    using MediatR;

    public class GetPermissionsQuery : IRequest<IEnumerable<PermissionDto>>
    {
    }
}
