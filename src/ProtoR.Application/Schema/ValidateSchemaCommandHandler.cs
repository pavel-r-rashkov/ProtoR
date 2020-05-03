namespace ProtoR.Application.Schema
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;

    public class ValidateSchemaCommandHandler : IRequestHandler<ValidateSchemaCommand, SchemaValidationResultDto>
    {
        private readonly IMapper mapper;
        private readonly IProtoBufSchemaGroupRepository groupRepository;
        private readonly IConfigurationRepository configurationRepository;

        public ValidateSchemaCommandHandler(
            IMapper mapper,
            IProtoBufSchemaGroupRepository groupRepository,
            IConfigurationRepository configurationRepository)
        {
            this.mapper = mapper;
            this.groupRepository = groupRepository;
            this.configurationRepository = configurationRepository;
        }

        public async Task<SchemaValidationResultDto> Handle(ValidateSchemaCommand request, CancellationToken cancellationToken)
        {
            var group = await this.groupRepository.GetByName(request.GroupName);
            var configuration = await this.configurationRepository.GetBySchemaGroupId(group.Id);
            var globalConfiguration = await this.configurationRepository.GetBySchemaGroupId(null);
            IEnumerable<RuleViolation> violations;
            ProtoBufSchema newSchema;

            try
            {
                (violations, newSchema) = group.TestSchema(
                    request.Contents,
                    this.GetGroupConfiguration(configuration, globalConfiguration),
                    configuration.MergeRuleConfiguration(globalConfiguration));
            }
            catch (InvalidProtoBufSchemaException exception)
            {
                return new SchemaValidationResultDto
                {
                    SchemaParseErrors = string.Join(Environment.NewLine, exception.Errors.Select(e => e.Message)),
                };
            }

            return new SchemaValidationResultDto
            {
                NewVersion = newSchema.Version.ToString(),
                RuleViolations = this.mapper.Map<IEnumerable<RuleViolationDto>>(violations),
            };
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
