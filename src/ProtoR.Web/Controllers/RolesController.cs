namespace ProtoR.Web.Controllers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using ProtoR.Application.Role;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Web.Infrastructure.Identity;
    using ProtoR.Web.Resources;
    using ProtoR.Web.Resources.RoleResource;
    using Permission = ProtoR.Web.Infrastructure.Identity.Permission;

    public class RolesController : BaseController
    {
        private readonly RoleManager<Role> manager;

        public RolesController(
            IMediator mediator,
            IMapper mapper,
            RoleManager<Role> manager)
            : base(mediator, mapper)
        {
            this.manager = manager;
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
        public async Task<ActionResult<ResponseModel<IEnumerable<RoleReadModel>>>> Get()
        {
            var roles = await this.Mediator.Send(new GetRolesQuery());
            var roleResources = this.Mapper.Map<IEnumerable<RoleReadModel>>(roles);

            return this.Ok(new ResponseModel<IEnumerable<RoleReadModel>>(roleResources));
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
        /// <response code="400">Role data is invalid.</response>
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">"RoleWrite" permission is missing.</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [HttpPost]
        [PermissionClaim(Permission.RoleWrite)]
        public async Task<ActionResult> Post([FromBody]RoleWriteModel role)
        {
            var newRole = new Role(role.Name, role.Permissions.Select(p => Domain.RoleAggregate.Permission.FromId(p)));
            await this.manager.CreateAsync(newRole);
            newRole = await this.manager.FindByNameAsync(newRole.Name);

            return this.CreatedAtAction(nameof(this.GetById), new { RoleId = newRole.Id }, null);
        }

        /// <summary>
        /// Update existing role.
        /// </summary>
        /// <returns>No content.</returns>
        /// <response code="204">Role updated successfully.</response>
        /// <response code="400">Role data is invalid.</response>
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">"RoleWrite" permission is missing.</response>
        /// <response code="404">Role with the specified ID doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpPut]
        [Route("{Id}")]
        [PermissionClaim(Permission.RoleWrite)]
        public async Task<ActionResult> Put(RoleWriteModel role)
        {
            var roleToUpdate = await this.manager.FindByIdAsync(role.Id.ToString(CultureInfo.InvariantCulture));
            this.Mapper.Map(role, roleToUpdate);
            await this.manager.UpdateAsync(roleToUpdate);

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
            var role = await this.manager.FindByIdAsync(roleId.ToString(CultureInfo.InvariantCulture));
            await this.manager.DeleteAsync(role);

            return this.NoContent();
        }
    }
}
