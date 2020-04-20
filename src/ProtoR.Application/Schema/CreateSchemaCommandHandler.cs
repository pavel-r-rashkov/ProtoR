namespace ProtoR.Application.Schema
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

    public class CreateSchemaCommandHandler : IRequestHandler<CreateSchemaCommand, string>
    {
        private readonly ISchemaGroupRepository<ProtoBufSchema, FileDescriptorSet> schemaGroupRepository;
        private readonly IConfigurationSetRepository configurationRepository;

        public CreateSchemaCommandHandler(
            ISchemaGroupRepository<ProtoBufSchema, FileDescriptorSet> schemaGroupRepository,
            IConfigurationSetRepository configurationRepository)
        {
            this.schemaGroupRepository = schemaGroupRepository;
            this.configurationRepository = configurationRepository;
        }

        public async Task<string> Handle(CreateSchemaCommand request, CancellationToken cancellationToken)
        {
            SchemaGroup<ProtoBufSchema, FileDescriptorSet> schemaGroup = await this.schemaGroupRepository.GetByName(request.GroupName);
            ConfigurationSet configuration = await this.configurationRepository.GetBySchemaGroupId(schemaGroup.Id);
            IEnumerable<RuleViolation> violations = schemaGroup.AddSchema(request.Contents, configuration, new ProtoBufSchemaFactory());

            if (!violations.Any(v => v.Severity.IsFatal))
            {
                await this.schemaGroupRepository.Update(schemaGroup);
            }

            // TODO return violations
            return schemaGroup.Schemas.ToList().Last().Version.ToString();
        }
    }
}
