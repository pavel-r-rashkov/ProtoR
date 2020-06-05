namespace ProtoR.Web.Controllers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using ProtoR.Application.User;
    using ProtoR.Domain.UserAggregate;
    using ProtoR.Web.Infrastructure.Identity;
    using ProtoR.Web.Resources.UserResource;

    public class UsersController : BaseController
    {
        private readonly UserManager<User> userManager;

        public UsersController(
            IMediator mediator,
            IMapper mapper,
            UserManager<User> userManager)
            : base(mediator, mapper)
        {
            this.userManager = userManager;
        }

        [HttpGet]
        [PermissionClaim(Permission.UserRead)]
        public async Task<ActionResult<IEnumerable<UserReadModel>>> Get([FromRoute]GetUsersQuery query)
        {
            var users = await this.Mediator.Send(query);
            var userResources = this.Mapper.Map<IEnumerable<UserReadModel>>(users);

            return this.Ok(userResources);
        }

        [HttpGet]
        [Route("{UserId}")]
        [PermissionClaim(Permission.UserRead)]
        public async Task<ActionResult<UserReadModel>> Get([FromRoute]GetUserByIdQuery query)
        {
            var users = await this.Mediator.Send(query);
            var userResource = this.Mapper.Map<UserReadModel>(users);

            return this.Ok(userResource);
        }

        [HttpPost]
        [PermissionClaim(Permission.UserWrite)]
        public async Task<ActionResult> Post(UserPostModel user)
        {
            var newUser = new User(user.UserName);
            var result = await this.userManager.CreateAsync(newUser, user.Password);

            if (!result.Succeeded)
            {
                return this.BadRequest(result.Errors);
            }

            var userId = await this.userManager.FindByNameAsync(user.UserName);

            return this.CreatedAtAction(nameof(this.Get), new { UserId = userId }, null);
        }

        [HttpPut]
        [Route("{Id}")]
        [PermissionClaim(Permission.UserWrite)]
        public async Task<ActionResult> Put(UserPutModel user)
        {
            var existingUser = await this.userManager.FindByIdAsync(user.Id.ToString(CultureInfo.InvariantCulture));
            existingUser.SetCategories(user.Categories);
            existingUser.SetRoles(user.Roles);
            var result = await this.userManager.UpdateAsync(existingUser);

            if (!result.Succeeded)
            {
                return this.BadRequest(result.Errors);
            }

            return this.NoContent();
        }

        [HttpDelete]
        [Route("{userId}")]
        [PermissionClaim(Permission.UserWrite)]
        public async Task<ActionResult> Delete(long userId)
        {
            var user = await this.userManager.FindByIdAsync(userId.ToString(CultureInfo.InvariantCulture));
            await this.userManager.DeleteAsync(user);

            return this.NoContent();
        }
    }
}
