namespace ProtoR.Application.Permission
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Domain.SeedWork;

    public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, IEnumerable<PermissionDto>>
    {
        private readonly IMapper mapper;

        public GetPermissionsQueryHandler(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public Task<IEnumerable<PermissionDto>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissions = Enumeration.GetAll<Permission>();
            var permissionsDto = this.mapper.Map<IEnumerable<PermissionDto>>(permissions);

            return Task.FromResult(permissionsDto);
        }
    }
}
