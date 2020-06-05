namespace ProtoR.Application.Category
{
    using System.Collections.Generic;
    using MediatR;

    public class GetCategoriesQuery : IRequest<IEnumerable<CategoryDto>>
    {
    }
}
