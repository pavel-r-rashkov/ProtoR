namespace ProtoR.Application.Category
{
    using MediatR;

    public class GetCategoryByIdQuery : IRequest<CategoryDto>
    {
        public string CategoryId { get; set; }
    }
}
