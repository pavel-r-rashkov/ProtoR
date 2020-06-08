namespace ProtoR.Application.Role
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Domain.Exceptions;
    using ProtoR.Domain.RoleAggregate;

    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleDto>
    {
        private readonly IRoleDataProvider roleData;

        public GetRoleByIdQueryHandler(IRoleDataProvider roleData)
        {
            this.roleData = roleData;
        }

        public async Task<RoleDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await this.roleData.GetById(request.RoleId);

            if (role == null)
            {
                throw new EntityNotFoundException<Role>(request.RoleId);
            }

            return role;
        }
    }
}
