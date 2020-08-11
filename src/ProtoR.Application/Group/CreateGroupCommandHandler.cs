namespace ProtoR.Application.Group
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Application.Common;
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.Exceptions;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SeedWork;

    public class CreateGroupCommandHandler : AsyncRequestHandler<CreateGroupCommand>
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

        protected override async Task Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            var existingGroup = await this.schemaGroupRepository.GetByName(request.Name);

            if (existingGroup != null)
            {
                throw new DuplicateGroupException(
                    $"Cannot create group with name {request.Name}. Group with that name already exists.",
                    request.Name);
            }

            var groupRestrictions = this.userProvider.GetGroupRestrictions();

            if (groupRestrictions != null)
            {
                var regex = FilterGenerator.CreateFromPatterns(groupRestrictions);
                var hasAccessToGroup = Regex.IsMatch(request.Name, regex, RegexOptions.IgnoreCase);

                if (!hasAccessToGroup)
                {
                    throw new InaccessibleGroupException(
                        request.Name,
                        this.userProvider.GetCurrentUserName());
                }
            }

            var group = new ProtoBufSchemaGroup(request.Name);
            long groupId = await this.schemaGroupRepository.Add(group);

            Configuration defaultConfiguration = Configuration.DefaultGroupConfiguration(groupId);
            await this.configurationRepository.Add(defaultConfiguration);

            await this.unitOfWork.SaveChanges();
        }
    }
}
