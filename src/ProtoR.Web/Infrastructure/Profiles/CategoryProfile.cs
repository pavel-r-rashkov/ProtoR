namespace ProtoR.Web.Infrastructure.Profiles
{
    using AutoMapper;
    using ProtoR.Application.Category;
    using ProtoR.Web.Resources.CategoryResource;

    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            this.CreateMap<CategoryDto, CategoryReadModel>();

            this.CreateMap<CategoryWriteModel, CreateCategoryCommand>();

            this.CreateMap<CategoryWriteModel, UpdateCategoryCommand>();
        }
    }
}
