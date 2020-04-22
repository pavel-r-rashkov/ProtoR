namespace ProtoR.Application.Group
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;

    public class CreateGroupCommandHandler : AsyncRequestHandler<CreateGroupCommand>
    {
        private readonly IProtoBufSchemaGroupRepository schemaGroupRepository;
        private readonly IConfigurationRepository configurationRepository;

        public CreateGroupCommandHandler(
            IProtoBufSchemaGroupRepository schemaGroupRepository,
            IConfigurationRepository configurationRepository)
        {
            this.schemaGroupRepository = schemaGroupRepository;
            this.configurationRepository = configurationRepository;
        }

        protected override async Task Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            var group = new ProtoBufSchemaGroup(request.Name);
            long groupId = await this.schemaGroupRepository.Add(group);

            Configuration defaultConfiguration = CreateDefaultConfiguration(groupId);
            await this.configurationRepository.Add(defaultConfiguration);
        }

        private static Dictionary<RuleCode, RuleConfiguration> CreateDefaultRulesConfiguration()
        {
            return RuleFactory.GetProtoBufRules().ToDictionary(
                r => r.Code,
                r => new RuleConfiguration(true, Severity.Hidden));
        }

        private static Configuration CreateDefaultConfiguration(long groupId)
        {
            return new Configuration(
                default,
                CreateDefaultRulesConfiguration(),
                groupId,
                new GroupConfiguration(false, true, false, true));
        }
    }
}
