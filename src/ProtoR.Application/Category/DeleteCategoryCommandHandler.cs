namespace ProtoR.Application.Category
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Domain.CategoryAggregate;
    using ProtoR.Domain.Exceptions;
    using ProtoR.Domain.SeedWork;

    public class DeleteCategoryCommandHandler : AsyncRequestHandler<DeleteCategoryCommand>
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IUnitOfWork unitOfWork;

        public DeleteCategoryCommandHandler(
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork)
        {
            this.categoryRepository = categoryRepository;
            this.unitOfWork = unitOfWork;
        }

        protected override async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await this.categoryRepository.GetById(request.CategoryId);

            if (category == null)
            {
                throw new EntityNotFoundException<Category>(request.CategoryId);
            }

            // TODO groups assigned to deleted category
            await this.categoryRepository.Delete(request.CategoryId);
            await this.unitOfWork.SaveChanges();
        }
    }
}
