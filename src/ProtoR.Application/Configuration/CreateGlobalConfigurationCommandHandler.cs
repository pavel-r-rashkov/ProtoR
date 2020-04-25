namespace ProtoR.Application.Configuration
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.SeedWork;

    public class CreateGlobalConfigurationCommandHandler : AsyncRequestHandler<CreateGlobalConfigurationCommand>
    {
        private readonly IConfigurationRepository configurationRepository;
        private readonly IUnitOfWork unitOfWork;

        public CreateGlobalConfigurationCommandHandler(
            IConfigurationRepository configurationRepository,
            IUnitOfWork unitOfWork)
        {
            this.configurationRepository = configurationRepository;
            this.unitOfWork = unitOfWork;
        }

        protected async override Task Handle(CreateGlobalConfigurationCommand command, CancellationToken cancellationToken)
        {
            Configuration globalConfiguration = await this.configurationRepository.GetBySchemaGroupId(null);

            if (globalConfiguration == null)
            {
                var configuration = Configuration.DefaultGlobalConfiguration();
                await this.configurationRepository.Add(configuration);
                await this.unitOfWork.SaveChanges();
            }
        }
    }
}
