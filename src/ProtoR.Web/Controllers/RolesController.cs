namespace ProtoR.Web.Controllers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using HybridModelBinding;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using ProtoR.Application.Role;
    using ProtoR.Domain.RoleAggregate;
    using ProtoR.Web.Infrastructure.Identity;
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

        [HttpGet]
        [PermissionClaim(Permission.RoleRead)]
        public async Task<ActionResult<IEnumerable<RoleReadModel>>> Get()
        {
            var roles = await this.Mediator.Send(new GetRolesQuery());
            var roleResources = this.Mapper.Map<IEnumerable<RoleReadModel>>(roles);

            return this.Ok(roleResources);
        }

        [HttpGet]
        [Route("{RoleId}")]
        [PermissionClaim(Permission.RoleRead)]
        public async Task<ActionResult<RoleReadModel>> GetById([FromRoute]GetRoleByIdQuery query)
        {
            var role = await this.Mediator.Send(query);
            var roleResource = this.Mapper.Map<RoleReadModel>(role);

            return this.Ok(roleResource);
        }

        [HttpPost]
        [PermissionClaim(Permission.RoleWrite)]
        public async Task<ActionResult> Post([FromBody]RoleWriteModel role)
        {
            var newRole = new Role(role.Name, role.Permissions.Select(p => Domain.RoleAggregate.Permission.FromId(p)));
            await this.manager.CreateAsync(newRole);
            newRole = await this.manager.FindByNameAsync(newRole.Name);

            return this.CreatedAtAction(nameof(this.GetById), new { RoleId = newRole.Id }, null);
        }

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
