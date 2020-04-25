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
    using ProtoR.Domain.SeedWork;

    public class CreateSchemaCommandHandler : IRequestHandler<CreateSchemaCommand, CreateSchemaCommandResult>
    {
        private readonly IProtoBufSchemaGroupRepository schemaGroupRepository;
        private readonly IConfigurationRepository configurationRepository;
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

        public CreateSchemaCommandHandler(
            IMapper mapper,
            IProtoBufSchemaGroupRepository schemaGroupRepository,
            IConfigurationRepository configurationRepository,
            IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.schemaGroupRepository = schemaGroupRepository;
            this.configurationRepository = configurationRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<CreateSchemaCommandResult> Handle(CreateSchemaCommand request, CancellationToken cancellationToken)
        {
            ProtoBufSchemaGroup schemaGroup = await this.schemaGroupRepository.GetByName(request.GroupName);
            Configuration configuration = await this.configurationRepository.GetBySchemaGroupId(schemaGroup.Id);
            Configuration globalConfiguration = await this.configurationRepository.GetBySchemaGroupId(null);
            IEnumerable<RuleViolation> violations;

            try
            {
                violations = schemaGroup.AddSchema(
                    request.Contents,
                    this.GetGroupConfiguration(configuration, globalConfiguration),
                    configuration.MergeRuleConfiguration(globalConfiguration));
            }
            catch (InvalidProtoBufSchemaException exception)
            {
                return new CreateSchemaCommandResult
                {
                    SchemaParseErrors = string.Join(Environment.NewLine, exception.Errors.Select(e => e.Message)),
                };
            }

            if (!violations.Any(v => v.Severity.IsFatal))
            {
                await this.schemaGroupRepository.Update(schemaGroup);
                await this.unitOfWork.SaveChanges();
            }

            var violationsDto = this.mapper.Map<IEnumerable<RuleViolationDto>>(violations);

            return new CreateSchemaCommandResult
            {
                NewVersion = schemaGroup.Schemas.ToList().Last().Version.ToString(),
                RuleViolations = violationsDto,
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
