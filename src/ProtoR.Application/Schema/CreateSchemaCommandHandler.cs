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
    using ProtoR.Domain.Exceptions;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.SeedWork;

    public class CreateSchemaCommandHandler : IRequestHandler<CreateSchemaCommand, SchemaValidationResultDto>
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

        public async Task<SchemaValidationResultDto> Handle(CreateSchemaCommand request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            ProtoBufSchemaGroup schemaGroup = await this.schemaGroupRepository.GetByName(request.GroupName);

            if (schemaGroup == null)
            {
                throw new EntityNotFoundException<ProtoBufSchemaGroup>(request.GroupName);
            }

            Configuration configuration = await this.configurationRepository.GetBySchemaGroupId(schemaGroup.Id);
            Configuration globalConfiguration = await this.configurationRepository.GetBySchemaGroupId(null);
            IEnumerable<RuleViolation> violations;

            violations = schemaGroup.AddSchema(
                request.Contents,
                this.GetGroupConfiguration(configuration, globalConfiguration),
                configuration.MergeRuleConfiguration(globalConfiguration));

            if (!violations.Any(v => v.Severity.IsFatal))
            {
                await this.schemaGroupRepository.Update(schemaGroup);
                await this.unitOfWork.SaveChanges();
            }

            var violationsDto = this.mapper.Map<IEnumerable<RuleViolationDto>>(violations);

            return new SchemaValidationResultDto
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
