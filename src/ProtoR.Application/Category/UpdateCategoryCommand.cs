namespace ProtoR.Application.Category
{
    using MediatR;

    public class UpdateCategoryCommand : IRequest
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }
}
