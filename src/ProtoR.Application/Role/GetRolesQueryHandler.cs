namespace ProtoR.Application.Role
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Application.Common;

    public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, PagedResult<RoleDto>>
    {
        private readonly IRoleDataProvider roleData;

        public GetRolesQueryHandler(IRoleDataProvider roleData)
        {
            this.roleData = roleData;
        }

        public async Task<PagedResult<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            return await this.roleData.GetRoles(
                request.Filter,
                request.OrderBy,
                request.Pagination);
        }
    }
}
