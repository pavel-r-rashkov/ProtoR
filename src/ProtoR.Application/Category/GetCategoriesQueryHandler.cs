namespace ProtoR.Application.Category
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;

    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IEnumerable<CategoryDto>>
    {
        private readonly ICategoryDataProvider categoryData;

        public GetCategoriesQueryHandler(ICategoryDataProvider categoryData)
        {
            this.categoryData = categoryData;
        }

        public async Task<IEnumerable<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await this.categoryData.GetCategories();
        }
    }
}
