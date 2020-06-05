namespace ProtoR.Application.Category
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Domain.CategoryAggregate;

    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
    {
        private readonly ICategoryDataProvider categoryData;

        public GetCategoryByIdQueryHandler(ICategoryDataProvider categoryData)
        {
            this.categoryData = categoryData;
        }

        public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var categoryId = request.CategoryId.Equals("default", StringComparison.InvariantCultureIgnoreCase)
                ? Category.DefaultCategoryId
                : Convert.ToInt64(request.CategoryId, CultureInfo.InvariantCulture);

            return await this.categoryData.GetById(categoryId);
        }
    }
}
