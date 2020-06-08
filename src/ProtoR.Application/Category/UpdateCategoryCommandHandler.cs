namespace ProtoR.Application.Category
{
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

        protected override async Task Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            var category = await this.categoryRepository.GetById(command.Id);

            if (category == null)
            {
                throw new EntityNotFoundException<Category>(command.Id);
            }

            var existingCategory = await this.categoryRepository.GetByName(command.Name);

            if (existingCategory != null)
            {
                throw new DuplicateCategoryException(
                    $"Cannot update name of category {category.Id} to {command.Name}",
                    command.Name);
            }

            category.Name = command.Name;
            await this.categoryRepository.Update(category);
            await this.unitOfWork.SaveChanges();
        }
    }
}
