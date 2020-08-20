namespace ProtoR.Web.Controllers
{
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using ProtoR.Application.Common;
    using ProtoR.Application.Role;
    using ProtoR.Web.Infrastructure.Identity;
    using ProtoR.Web.Resources;
    using ProtoR.Web.Resources.RoleResource;
    using Permission = ProtoR.Web.Infrastructure.Identity.Permission;

    public class RolesController : BaseController
    {
        public RolesController(
            IMediator mediator,
            IMapper mapper)
            : base(mediator, mapper)
        {
        }

        /// <summary>
        /// Get all roles.
        /// </summary>
        /// <returns>Roles.</returns>
        /// <response code="200">Roles list.</response>
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">"RoleRead" permission is missing.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [HttpGet]
        [PermissionClaim(Permission.RoleRead)]
        public async Task<ActionResult<ResponseModel<PagedResult<RoleReadModel>>>> Get([FromQuery]GetRolesQuery query)
        {
            var roles = await this.Mediator.Send(query);
            var roleResources = this.Mapper.Map<PagedResult<RoleReadModel>>(roles);

            return this.Ok(new ResponseModel<PagedResult<RoleReadModel>>(roleResources));
        }

        /// <summary>
        /// Get role by ID.
        /// </summary>
        /// <returns>Role.</returns>
        /// <response code="200">Role with requested ID.</response>
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">"RoleRead" permission is missing.</response>
        /// <response code="404">Role with the specified ID doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{RoleId}")]
        [PermissionClaim(Permission.RoleRead)]
        public async Task<ActionResult<ResponseModel<RoleReadModel>>> GetById([FromRoute]GetRoleByIdQuery query)
        {
            var role = await this.Mediator.Send(query);
            var roleResource = this.Mapper.Map<RoleReadModel>(role);

            return this.Ok(new ResponseModel<RoleReadModel>(roleResource));
        }

        /// <summary>
        /// Create a new role.
        /// </summary>
        /// <returns>No content.</returns>
        /// <response code="201">Role created successfully.</response>
        /// <response code="422">Role data is invalid.</response>
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">"RoleWrite" permission is missing.</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [HttpPost]
        [PermissionClaim(Permission.RoleWrite)]
        public async Task<ActionResult> Post([FromBody]RoleWriteModel role)
        {
            var command = this.Mapper.Map<CreateRoleCommand>(role);
            var roleId = await this.Mediator.Send(command);

            return this.CreatedAtAction(nameof(this.GetById), new { RoleId = roleId }, null);
        }

        /// <summary>
        /// Update existing role.
        /// </summary>
        /// <returns>No content.</returns>
        /// <response code="204">Role updated successfully.</response>
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">"RoleWrite" permission is missing.</response>
        /// <response code="404">Role with the specified ID doesn't exist.</response>
        /// <response code="422">Role data is invalid.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpPut]
        [Route("{Id}")]
        [PermissionClaim(Permission.RoleWrite)]
        public async Task<ActionResult> Put(RoleWriteModel role)
        {
            var command = this.Mapper.Map<UpdateRoleCommand>(role);
            await this.Mediator.Send(command);

            return this.NoContent();
        }

        /// <summary>
        /// Delete a role.
        /// </summary>
        /// <returns>No conent.</returns>
        /// <response code="204">Role deleted successfully.</response>
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">"RoleWrite" permission is missing.</response>
        /// <response code="404">Role with the specified ID doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpDelete]
        [Route("{roleId}")]
        [PermissionClaim(Permission.RoleWrite)]
        public async Task<ActionResult> Delete(long roleId)
        {
            await this.Mediator.Send(new DeleteRoleCommand { RoleId = roleId });

            return this.NoContent();
        }
    }
}
