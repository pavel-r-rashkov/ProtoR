namespace ProtoR.Application.Schema
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using MediatR;
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.Exceptions;
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
            _ = request ?? throw new ArgumentNullException(nameof(request));
            var group = await this.groupRepository.GetByName(request.GroupName);

            if (group == null)
            {
                throw new EntityNotFoundException<ProtoBufSchemaGroup>(request.GroupName);
            }

            var configuration = await this.configurationRepository.GetBySchemaGroupId(group.Id);
            var globalConfiguration = await this.configurationRepository.GetBySchemaGroupId(null);
            IEnumerable<RuleViolation> violations;
            ProtoBufSchema newSchema;

            (violations, newSchema) = group.TestSchema(
                request.Contents,
                this.GetGroupConfiguration(configuration, globalConfiguration),
                configuration.MergeRuleConfiguration(globalConfiguration));

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
