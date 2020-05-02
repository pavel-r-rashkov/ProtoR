namespace ProtoR.Application.Group
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SeedWork;

    public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, bool>
    {
        private readonly IProtoBufSchemaGroupRepository schemaGroupRepository;
        private readonly IConfigurationRepository configurationRepository;
        private readonly IUnitOfWork unitOfWork;

        public CreateGroupCommandHandler(
            IProtoBufSchemaGroupRepository schemaGroupRepository,
            IConfigurationRepository configurationRepository,
            IUnitOfWork unitOfWork)
        {
            this.schemaGroupRepository = schemaGroupRepository;
            this.configurationRepository = configurationRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            var existingGroup = await this.schemaGroupRepository.GetByName(request.Name);

            if (existingGroup != null)
            {
                return false;
            }

            var group = new ProtoBufSchemaGroup(request.Name);
            long groupId = await this.schemaGroupRepository.Add(group);

            Configuration defaultConfiguration = Configuration.DefaultGroupConfiguration(groupId);
            await this.configurationRepository.Add(defaultConfiguration);
            await this.unitOfWork.SaveChanges();
            return true;
        }
    }
}
