namespace ProtoR.Application.Schema
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Google.Protobuf.Reflection;
    using MediatR;
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;

    public class CreateSchemaCommandHandler : IRequestHandler<CreateSchemaCommand, string>
    {
        private readonly IProtoBufSchemaGroupRepository schemaGroupRepository;
        private readonly IConfigurationRepository configurationRepository;

        public CreateSchemaCommandHandler(
            IProtoBufSchemaGroupRepository schemaGroupRepository,
            IConfigurationRepository configurationRepository)
        {
            this.schemaGroupRepository = schemaGroupRepository;
            this.configurationRepository = configurationRepository;
        }

        public async Task<string> Handle(CreateSchemaCommand request, CancellationToken cancellationToken)
        {
            ProtoBufSchemaGroup schemaGroup = await this.schemaGroupRepository.GetByName(request.GroupName);
            Configuration configuration = await this.configurationRepository.GetBySchemaGroupId(schemaGroup.Id);
            Configuration globalConfiguration = await this.configurationRepository.GetBySchemaGroupId(null);

            IEnumerable<RuleViolation> violations = schemaGroup.AddSchema(
                request.Contents,
                this.GetGroupConfiguration(configuration, globalConfiguration),
                configuration.MergeRuleConfiguration(globalConfiguration));

            if (!violations.Any(v => v.Severity.IsFatal))
            {
                await this.schemaGroupRepository.Update(schemaGroup);
            }

            // TODO return violations
            return schemaGroup.Schemas.ToList().Last().Version.ToString();
        }

        private GroupConfiguration GetGroupConfiguration(
            Configuration configuration,
            Configuration globalConfiguration)
        {
            return configuration.GroupConfiguration.Inherit
                ? globalConfiguration.GroupConfiguration
                : configuration.GroupConfiguration;
        }
    }
}
