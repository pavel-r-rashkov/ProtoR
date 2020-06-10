namespace ProtoR.Application.Category
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Domain.CategoryAggregate;
    using ProtoR.Domain.Exceptions;
    using ProtoR.Domain.SeedWork;

    public class UpdateCategoryCommandHandler : AsyncRequestHandler<UpdateCategoryCommand>
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IUnitOfWork unitOfWork;

        public UpdateCategoryCommandHandler(
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork)
        {
            this.categoryRepository = categoryRepository;
            this.unitOfWork = unitOfWork;
        }

        protected override async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            var category = await this.categoryRepository.GetById(request.Id);

            if (category == null)
            {
                throw new EntityNotFoundException<Category>(request.Id);
            }

            var existingCategory = await this.categoryRepository.GetByName(request.Name);

            if (existingCategory != null)
            {
                throw new DuplicateCategoryException(
                    $"Cannot update name of category {category.Id} to {request.Name}",
                    request.Name);
            }

            category.Name = request.Name;
            await this.categoryRepository.Update(category);
            await this.unitOfWork.SaveChanges();
        }
    }
}
