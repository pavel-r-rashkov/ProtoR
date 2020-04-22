namespace ProtoR.Application.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using ProtoR.Domain.SeedWork;

    public class UpdateConfigurationCommandHandler : AsyncRequestHandler<UpdateConfigurationCommand>
    {
        private readonly IConfigurationRepository configurationRepository;

        public UpdateConfigurationCommandHandler(IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
        }

        protected override async Task Handle(UpdateConfigurationCommand request, CancellationToken cancellationToken)
        {
            Configuration configuration = request.ConfigurationId.Equals("global", StringComparison.InvariantCultureIgnoreCase)
                ? await this.configurationRepository.GetBySchemaGroupId(null)
                : await this.configurationRepository.GetById(Convert.ToInt64(request.ConfigurationId, CultureInfo.InvariantCulture));

            configuration.SetGroupConfiguration(new GroupConfiguration(
                request.ForwardCompatible,
                request.BackwardCompatible,
                request.Transitive,
                request.Inherit));

            Dictionary<int, Severity> severities = Enumeration
                .GetAll<Severity>()
                .ToDictionary(s => s.Id);

            Dictionary<RuleCode, RuleConfiguration> rulesConfiguration = request.RuleConfigurations.ToDictionary(
                r => (RuleCode)Enum.Parse(typeof(RuleCode), r.RuleCode),
                r => new RuleConfiguration(r.Inherit, severities[r.Severity]));

            configuration.SetRulesConfiguration(rulesConfiguration);

            await this.configurationRepository.Update(configuration);
        }
    }
}
