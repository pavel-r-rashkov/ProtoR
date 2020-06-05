namespace ProtoR.Web.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using ProtoR.Application.Permission;
    using ProtoR.Web.Infrastructure.Identity;
    using ProtoR.Web.Resources.PermissionResource;

    public class PermissionsController : BaseController
    {
        public PermissionsController(
            IMediator mediator,
            IMapper mapper)
            : base(mediator, mapper)
        {
        }

        [HttpGet]
        [PermissionClaim(Permission.PermissionRead)]
        public async Task<ActionResult<IEnumerable<PermissionReadModel>>> Get()
        {
            IEnumerable<PermissionDto> permissions = await this.Mediator.Send(new GetPermissionsQuery());
            var permissionResources = this.Mapper.Map<IEnumerable<PermissionReadModel>>(permissions);

            return this.Ok(permissionResources);
        }
    }
}
