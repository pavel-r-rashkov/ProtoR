namespace ProtoR.Web.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using ProtoR.Application.Category;
    using ProtoR.Web.Infrastructure.Identity;
    using ProtoR.Web.Resources;
    using ProtoR.Web.Resources.CategoryResource;

    public class CategoriesController : BaseController
    {
        public CategoriesController(
            IMediator mediator,
            IMapper mapper)
            : base(mediator, mapper)
        {
        }

        /// <summary>
        /// Get all categories.
        /// </summary>
        /// <returns>Category list.</returns>
        /// <response code="200">All categories.</response>
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">"CategoryRead" permission is missing.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [HttpGet]
        [PermissionClaim(Permission.CategoryRead)]
        public async Task<ActionResult<ResponseModel<IEnumerable<CategoryReadModel>>>> Get([FromRoute]GetCategoriesQuery query)
        {
            var categories = await this.Mediator.Send(query);
            var categoryResources = this.Mapper.Map<IEnumerable<CategoryReadModel>>(categories);

            return this.Ok(new ResponseModel<IEnumerable<CategoryReadModel>>(categoryResources));
        }

        /// <summary>
        /// Get category by ID.
        /// </summary>
        /// <returns>Category.</returns>
        /// <response code="200">Category with requested ID.</response>
        /// <response code="400">Category ID is invalid.</response>
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">"CategoryRead" permission is missing.</response>
        /// <response code="404">Category with the specified ID doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpGet]
        [Route("{CategoryId}")]
        [PermissionClaim(Permission.CategoryRead)]
        public async Task<ActionResult<ResponseModel<CategoryReadModel>>> Get([FromRoute]GetCategoryByIdQuery query)
        {
            var category = await this.Mediator.Send(query);
            var categoryResource = this.Mapper.Map<CategoryReadModel>(category);

            return this.Ok(new ResponseModel<CategoryReadModel>(categoryResource));
        }

        /// <summary>
        /// Create new category.
        /// </summary>
        /// <returns>No content.</returns>
        /// <response code="201">Category created successfully.</response>
        /// <response code="400">Category data is invalid.</response>
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">"CategoryWrite" permission is missing.</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [HttpPost]
        [PermissionClaim(Permission.CategoryWrite)]
        public async Task<ActionResult> Post(CategoryWriteModel category)
        {
            var command = this.Mapper.Map<CreateCategoryCommand>(category);
            var categoryId = await this.Mediator.Send(command);

            return this.CreatedAtAction(nameof(this.Get), new { CategoryId = categoryId }, null);
        }

        /// <summary>
        /// Update existing category.
        /// </summary>
        /// <returns>No content.</returns>
        /// <response code="204">Category updated successfully.</response>
        /// <response code="400">Category data is invalid.</response>
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">"CategoryWrite" permission is missing.</response>
        /// <response code="404">Category with the specified ID doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpPut]
        [Route("{Id}")]
        [PermissionClaim(Permission.CategoryWrite)]
        public async Task<ActionResult> Put(CategoryWriteModel category)
        {
            var command = this.Mapper.Map<UpdateCategoryCommand>(category);
            await this.Mediator.Send(command);

            return this.NoContent();
        }

        /// <summary>
        /// Delete a category.
        /// </summary>
        /// <returns>No content.</returns>
        /// <response code="204">Category deleted successfully.</response>
        /// <response code="401">User or client is not authenticated.</response>
        /// <response code="403">"CategoryWrite" permission is missing.</response>
        /// <response code="404">Category with the specified ID doesn't exist.</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [HttpDelete]
        [Route("{categoryId}")]
        [PermissionClaim(Permission.CategoryWrite)]
        public async Task<ActionResult> Delete(long categoryId)
        {
            await this.Mediator.Send(new DeleteCategoryCommand { CategoryId = categoryId });

            return this.NoContent();
        }
    }
}
