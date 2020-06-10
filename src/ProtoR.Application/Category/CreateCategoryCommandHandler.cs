namespace ProtoR.Application.Category
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Domain.CategoryAggregate;
    using ProtoR.Domain.Exceptions;
    using ProtoR.Domain.SeedWork;

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, long>
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IUnitOfWork unitOfWork;

        public CreateCategoryCommandHandler(
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork)
        {
            this.categoryRepository = categoryRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<long> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            var existingCategory = await this.categoryRepository.GetByName(request.Name);

            if (existingCategory != null)
            {
                throw new DuplicateCategoryException(
                    $"Cannot create category with name {request.Name}. Category with that name already exists.",
                    request.Name);
            }

            var category = new Category(request.Name);
            var id = await this.categoryRepository.Add(category);
            await this.unitOfWork.SaveChanges();

            return id;
        }
    }
}
