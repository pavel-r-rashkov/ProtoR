namespace ProtoR.Web.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using ProtoR.Application.User;
    using ProtoR.Domain.UserAggregate;
    using ProtoR.Web.Infrastructure.Identity;
    using ProtoR.Web.Resources;
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

        /// <summary>
        /// Get all users.
        /// </summary>
        /// <returns>Users.</returns>
        /// <response code="200">Users list.</response>
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">"UserRead" permission is missing.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [HttpGet]
        [PermissionClaim(Permission.UserRead)]
        public async Task<ActionResult<ResponseModel<IEnumerable<UserReadModel>>>> Get([FromRoute]GetUsersQuery query)
        {
            var users = await this.Mediator.Send(query);
            var userResources = this.Mapper.Map<IEnumerable<UserReadModel>>(users);

            return this.Ok(new ResponseModel<IEnumerable<UserReadModel>>(userResources));
        }

        /// <summary>
        /// Get user by ID.
        /// </summary>
        /// <returns>User.</returns>
        /// <response code="200">User with requested ID.</response>
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">"UserRead" permission is missing.</response>
        /// <response code="404">User with the specified ID doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{UserId}")]
        [PermissionClaim(Permission.UserRead)]
        public async Task<ActionResult<ResponseModel<UserReadModel>>> Get([FromRoute]GetUserByIdQuery query)
        {
            var users = await this.Mediator.Send(query);
            var userResource = this.Mapper.Map<UserReadModel>(users);

            return this.Ok(new ResponseModel<UserReadModel>(userResource));
        }

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <returns>No content.</returns>
        /// <response code="201">User created successfully.</response>
        /// <response code="400">User data is invalid.</response>
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">"UserWrite" permission is missing.</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [HttpPost]
        [PermissionClaim(Permission.UserWrite)]
        public async Task<ActionResult> Post(UserPostModel user)
        {
            var command = this.Mapper.Map<CreateUserCommand>(user);
            var userId = await this.Mediator.Send(command);

            return this.CreatedAtAction(nameof(this.Get), new { UserId = userId }, null);
        }

        /// <summary>
        /// Update existing user.
        /// </summary>
        /// <returns>No content.</returns>
        /// <response code="204">User updated successfully.</response>
        /// <response code="400">User data is invalid.</response>
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">"UserWrite" permission is missing.</response>
        /// <response code="404">User with the specified ID doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpPut]
        [Route("{Id}")]
        [PermissionClaim(Permission.UserWrite)]
        public async Task<ActionResult> Put(UserPutModel user)
        {
            var command = this.Mapper.Map<UpdateUserCommand>(user);
            await this.Mediator.Send(command);

            return this.NoContent();
        }

        /// <summary>
        /// Delete a user.
        /// </summary>
        /// <returns>No conent.</returns>
        /// <response code="204">User deleted successfully.</response>
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">"UserWrite" permission is missing.</response>
        /// <response code="404">User with the specified ID doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpDelete]
        [Route("{userId}")]
        [PermissionClaim(Permission.UserWrite)]
        public async Task<ActionResult> Delete(long userId)
        {
            await this.Mediator.Send(new DeleteUserCommand { UserId = userId });

            return this.NoContent();
        }
    }
}
