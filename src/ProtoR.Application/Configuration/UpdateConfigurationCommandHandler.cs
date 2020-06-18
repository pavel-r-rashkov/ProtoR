namespace ProtoR.Application.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Application.Common;
    using ProtoR.Application.Group;
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.Exceptions;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using ProtoR.Domain.SeedWork;

    public class UpdateConfigurationCommandHandler : AsyncRequestHandler<UpdateConfigurationCommand>
    {
        private readonly IConfigurationRepository configurationRepository;
        private readonly IGroupDataProvider groupData;
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserProvider userProvider;

        public UpdateConfigurationCommandHandler(
            IConfigurationRepository configurationRepository,
            IGroupDataProvider groupData,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider)
        {
            this.configurationRepository = configurationRepository;
            this.groupData = groupData;
            this.unitOfWork = unitOfWork;
            this.userProvider = userProvider;
        }

        protected override async Task Handle(UpdateConfigurationCommand request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            Configuration configuration = request.ConfigurationId.Equals("global", StringComparison.InvariantCultureIgnoreCase)
                ? await this.configurationRepository.GetBySchemaGroupId(null)
                : await this.configurationRepository.GetById(Convert.ToInt64(request.ConfigurationId, CultureInfo.InvariantCulture));

            if (configuration == null)
            {
                throw new EntityNotFoundException<Configuration>((object)request.ConfigurationId);
            }

            var groupRestrictions = this.userProvider.GetGroupRestrictions();

            if (groupRestrictions != null && configuration.SchemaGroupId != null)
            {
                var groupName = await this.groupData.GetGroupNameById(configuration.SchemaGroupId.Value);
                var regex = FilterGenerator.CreateFromPatterns(groupRestrictions);
                var hasAccessToGroup = Regex.IsMatch(groupName, regex, RegexOptions.IgnoreCase);

                if (!hasAccessToGroup)
                {
                    throw new InaccessibleGroupException(
                        groupName,
                        this.userProvider.GetCurrentUserName());
                }
            }

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
            await this.unitOfWork.SaveChanges();
        }
    }
}
