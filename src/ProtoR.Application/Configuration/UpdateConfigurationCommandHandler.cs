namespace ProtoR.Application.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using ProtoR.Application.Group;
    using ProtoR.Domain.CategoryAggregate;
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using ProtoR.Domain.SeedWork;

    public class UpdateConfigurationCommandHandler : AsyncRequestHandler<UpdateConfigurationCommand>
    {
        private readonly IConfigurationRepository configurationRepository;
        private readonly IGroupDataProvider groupDataProvider;
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserProvider userProvider;

        public UpdateConfigurationCommandHandler(
            IConfigurationRepository configurationRepository,
            IGroupDataProvider groupDataProvider,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider)
        {
            this.configurationRepository = configurationRepository;
            this.groupDataProvider = groupDataProvider;
            this.unitOfWork = unitOfWork;
            this.userProvider = userProvider;
        }

        protected override async Task Handle(UpdateConfigurationCommand request, CancellationToken cancellationToken)
        {
            Configuration configuration = request.ConfigurationId.Equals("global", StringComparison.InvariantCultureIgnoreCase)
                ? await this.configurationRepository.GetBySchemaGroupId(null)
                : await this.configurationRepository.GetById(Convert.ToInt64(request.ConfigurationId, CultureInfo.InvariantCulture));

            if (configuration == null)
            {
                // TODO
                throw new Exception("Configuration not found");
            }

            var categories = this.userProvider.GetCategoryRestrictions();

            if (categories != null && configuration.SchemaGroupId != null)
            {
                var categoryId = await this.groupDataProvider.GetCategoryId(configuration.SchemaGroupId.Value);

                if (!categories.Contains(categoryId))
                {
                    throw new InaccessibleCategoryException(
                        categoryId,
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
