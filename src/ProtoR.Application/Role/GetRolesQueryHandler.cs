namespace ProtoR.Application.Role
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, IEnumerable<RoleDto>>
    {
        private readonly IRoleDataProvider roleData;

        public GetRolesQueryHandler(IRoleDataProvider roleData)
        {
            this.roleData = roleData;
        }

        public async Task<IEnumerable<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            return await this.roleData.GetRoles();
        }
    }
}
