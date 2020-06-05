namespace ProtoR.Application.Category
{
    using MediatR;

    public class CreateCategoryCommand : IRequest<long>
    {
        public string Name { get; set; }
    }
}
