namespace ProtoR.Application.Group
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Domain.CategoryAggregate;
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SeedWork;

    public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, bool>
    {
        private readonly IProtoBufSchemaGroupRepository schemaGroupRepository;
        private readonly IConfigurationRepository configurationRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserProvider userProvider;

        public CreateGroupCommandHandler(
            IProtoBufSchemaGroupRepository schemaGroupRepository,
            IConfigurationRepository configurationRepository,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider)
        {
            this.schemaGroupRepository = schemaGroupRepository;
            this.configurationRepository = configurationRepository;
            this.unitOfWork = unitOfWork;
            this.userProvider = userProvider;
        }

        public async Task<bool> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            var existingGroup = await this.schemaGroupRepository.GetByName(request.GroupName);

            if (existingGroup != null)
            {
                // TODO throw exception
                return false;
            }

            var categoryId = request.CategoryId ?? Category.DefaultCategoryId;
            var categories = this.userProvider.GetCategoryRestrictions();

            if (categories != null && !categories.Contains(categoryId))
            {
                throw new InaccessibleCategoryException(
                    categoryId,
                    this.userProvider.GetCurrentUserName());
            }

            var group = new ProtoBufSchemaGroup(request.GroupName, categoryId);
            long groupId = await this.schemaGroupRepository.Add(group);

            Configuration defaultConfiguration = Configuration.DefaultGroupConfiguration(groupId);
            await this.configurationRepository.Add(defaultConfiguration);

            await this.unitOfWork.SaveChanges();

            return true;
        }
    }
}