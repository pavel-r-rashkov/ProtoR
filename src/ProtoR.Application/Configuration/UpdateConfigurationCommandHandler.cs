namespace ProtoR.Application.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Domain.ConfigurationSetAggregate;
    using ProtoR.Domain.GlobalConfigurationAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using ProtoR.Domain.SeedWork;

    public class UpdateConfigurationCommandHandler : AsyncRequestHandler<UpdateConfigurationCommand>
    {
        private readonly IConfigurationSetRepository configurationRepository;

        public UpdateConfigurationCommandHandler(IConfigurationSetRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
        }

        protected override async Task Handle(UpdateConfigurationCommand request, CancellationToken cancellationToken)
        {
            ConfigurationSet configuration = request.ConfigurationId.Equals("global", StringComparison.InvariantCultureIgnoreCase)
                ? await this.configurationRepository.GetBySchemaGroupId(null)
                : await this.configurationRepository.GetById(Convert.ToInt64(request.ConfigurationId, CultureInfo.InvariantCulture));

            configuration.ShouldInherit = request.Inherit;

            if (!configuration.ShouldInherit)
            {
                configuration.SetCompatibility(request.BackwardCompatible, request.ForwardCompatible);
                configuration.Transitive = request.Transitive;
            }

            Dictionary<int, Severity> severities = Enumeration
                .GetAll<Severity>()
                .ToDictionary(s => s.Id);

            Dictionary<RuleCode, RuleConfig> rulesConfiguration = request.RuleConfigurations.ToDictionary(
                r => (RuleCode)Enum.Parse(typeof(RuleCode), r.RuleCode),
                r => new RuleConfig(r.Inherit, severities[r.Severity]));

            configuration.SetRulesConfiguration(rulesConfiguration);

            await this.configurationRepository.Update(configuration);
        }
    }
}
