namespace ProtoR.Application.Registry
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Domain.CategoryAggregate;
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.SeedWork;

    public class InitRegistryCommandHandler : AsyncRequestHandler<InitRegistryCommand>
    {
        private readonly IConfigurationRepository configurationRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IUnitOfWork unitOfWork;

        public InitRegistryCommandHandler(
            IConfigurationRepository configurationRepository,
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork)
        {
            this.configurationRepository = configurationRepository;
            this.categoryRepository = categoryRepository;
            this.unitOfWork = unitOfWork;
        }

        protected async override Task Handle(InitRegistryCommand request, CancellationToken cancellationToken)
        {
            var globalConfiguration = await this.configurationRepository.GetBySchemaGroupId(null);

            if (globalConfiguration == null)
            {
                var configuration = Configuration.DefaultGlobalConfiguration();
                await this.configurationRepository.Add(configuration);
            }

            var existingCategory = await this.categoryRepository.GetById(Category.DefaultCategoryId);

            if (existingCategory == null)
            {
                var category = Category.CreateDefault();
                await this.categoryRepository.Add(category);
            }

            await this.unitOfWork.SaveChanges();
        }
    }
}
