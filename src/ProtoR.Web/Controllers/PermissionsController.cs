namespace ProtoR.Web.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using ProtoR.Application.Permission;
    using ProtoR.Web.Infrastructure.Identity;
    using ProtoR.Web.Resources;
    using ProtoR.Web.Resources.PermissionResource;

    public class PermissionsController : BaseController
    {
        public PermissionsController(
            IMediator mediator,
            IMapper mapper)
            : base(mediator, mapper)
        {
        }

        /// <summary>
        /// Get all permissions.
        /// </summary>
        /// <returns>Permissions.</returns>
        /// <response code="200">Permissions list.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        [PermissionClaim(Permission.PermissionRead)]
        public async Task<ActionResult<ResponseModel<IEnumerable<PermissionReadModel>>>> Get()
        {
            IEnumerable<PermissionDto> permissions = await this.Mediator.Send(new GetPermissionsQuery());
            var permissionResources = this.Mapper.Map<IEnumerable<PermissionReadModel>>(permissions);

            return this.Ok(new ResponseModel<IEnumerable<PermissionReadModel>>(permissionResources));
        }
    }
}
