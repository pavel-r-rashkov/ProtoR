namespace ProtoR.Application.Group
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Google.Protobuf.Reflection;
    using MediatR;
    using ProtoR.Domain.ConfigurationSetAggregate;
    using ProtoR.Domain.GlobalConfigurationAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;

    public class CreateGroupCommandHandler : AsyncRequestHandler<CreateGroupCommand>
    {
        private readonly ISchemaGroupRepository<ProtoBufSchema, FileDescriptorSet> schemaGroupRepository;
        private readonly IConfigurationSetRepository configurationRepository;

        public CreateGroupCommandHandler(
            ISchemaGroupRepository<ProtoBufSchema, FileDescriptorSet> schemaGroupRepository,
            IConfigurationSetRepository configurationRepository)
        {
            this.schemaGroupRepository = schemaGroupRepository;
            this.configurationRepository = configurationRepository;
        }

        protected override async Task Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            var group = new SchemaGroup<ProtoBufSchema, FileDescriptorSet>(request.Name);
            long groupId = await this.schemaGroupRepository.Add(group);

            ConfigurationSet defaultConfiguration = CreateDefaultConfiguration(groupId);
            await this.configurationRepository.Add(defaultConfiguration);
        }

        private static Dictionary<RuleCode, RuleConfig> CreateDefaultRulesConfiguration()
        {
            return RuleFactory.GetProtoBufRules().ToDictionary(
                r => r.Code,
                r => new RuleConfig(true, Severity.Hidden));
        }

        private static ConfigurationSet CreateDefaultConfiguration(long groupId)
        {
            return new ConfigurationSet(
                default,
                CreateDefaultRulesConfiguration(),
                groupId,
                true,
                false,
                true,
                false);
        }
    }
}
