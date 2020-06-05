namespace ProtoR.Application.Role
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleDto>
    {
        private readonly IRoleDataProvider roleData;

        public GetRoleByIdQueryHandler(IRoleDataProvider roleData)
        {
            this.roleData = roleData;
        }

        public async Task<RoleDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            return await this.roleData.GetById(request.RoleId);
        }
    }
}
