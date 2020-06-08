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

        public async Task<long> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            var existingCategory = await this.categoryRepository.GetByName(command.Name);

            if (existingCategory != null)
            {
                throw new DuplicateCategoryException(
                    $"Cannot create category with name {command.Name}. Category with that name already exists.",
                    command.Name);
            }

            var category = new Category(command.Name);
            var id = await this.categoryRepository.Add(category);
            await this.unitOfWork.SaveChanges();

            return id;
        }
    }
}
