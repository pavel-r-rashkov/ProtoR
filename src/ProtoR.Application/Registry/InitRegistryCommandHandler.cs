namespace ProtoR.Application.Registry
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.SeedWork;

    public class InitRegistryCommandHandler : AsyncRequestHandler<InitRegistryCommand>
    {
        private readonly IConfigurationRepository configurationRepository;
        private readonly IUnitOfWork unitOfWork;

        public InitRegistryCommandHandler(
            IConfigurationRepository configurationRepository,
            IUnitOfWork unitOfWork)
        {
            this.configurationRepository = configurationRepository;
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

            await this.unitOfWork.SaveChanges();
        }
    }
}
