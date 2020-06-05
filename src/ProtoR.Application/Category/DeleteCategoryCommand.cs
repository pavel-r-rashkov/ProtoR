namespace ProtoR.Application.Category
{
    using MediatR;

    public class DeleteCategoryCommand : IRequest
    {
        public long CategoryId { get; set; }
    }
}
