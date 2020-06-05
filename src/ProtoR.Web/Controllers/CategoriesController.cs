namespace ProtoR.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using ProtoR.Application.Category;
    using ProtoR.Web.Infrastructure.Identity;
    using ProtoR.Web.Resources.CategoryResource;

    public class CategoriesController : BaseController
    {
        public CategoriesController(
            IMediator mediator,
            IMapper mapper)
            : base(mediator, mapper)
        {
        }

        [HttpGet]
        [PermissionClaim(Permission.CategoryRead)]
        public async Task<ActionResult<IEnumerable<CategoryReadModel>>> Get([FromRoute]GetCategoriesQuery query)
        {
            var categories = await this.Mediator.Send(query);
            var categoryResources = this.Mapper.Map<IEnumerable<CategoryReadModel>>(categories);

            return this.Ok(categoryResources);
        }

        [HttpGet]
        [Route("{CategoryId}")]
        [PermissionClaim(Permission.CategoryRead)]
        public async Task<ActionResult<CategoryReadModel>> Get([FromRoute]GetCategoryByIdQuery query)
        {
            var category = await this.Mediator.Send(query);
            var categoryResource = this.Mapper.Map<CategoryReadModel>(category);

            return this.Ok(categoryResource);
        }

        [HttpPost]
        [PermissionClaim(Permission.CategoryWrite)]
        public async Task<ActionResult> Post(CategoryWriteModel category)
        {
            var command = this.Mapper.Map<CreateCategoryCommand>(category);
            var categoryId = await this.Mediator.Send(command);

            return this.CreatedAtAction(nameof(this.Get), new { CategoryId = categoryId }, null);
        }

        [HttpPut]
        [Route("{Id}")]
        [PermissionClaim(Permission.CategoryWrite)]
        public async Task<ActionResult> Put(CategoryWriteModel category)
        {
            var command = this.Mapper.Map<UpdateCategoryCommand>(category);
            await this.Mediator.Send(command);

            return this.NoContent();
        }

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
