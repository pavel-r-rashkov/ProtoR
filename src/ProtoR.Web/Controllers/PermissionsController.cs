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
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">
        /// "PermissionRead" permission is missing.
        /// </response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
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
