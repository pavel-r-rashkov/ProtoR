namespace ProtoR.Application.Category
{
    using MediatR;

    public class GetCategoryByIdQuery : IRequest<CategoryDto>
    {
        /// <summary>
        /// Category ID.
        /// </summary>
        public string CategoryId { get; set; }
    }
}
