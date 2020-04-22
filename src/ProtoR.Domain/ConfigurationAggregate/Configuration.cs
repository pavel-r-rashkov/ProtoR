namespace ProtoR.Domain.ConfigurationAggregate
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using ProtoR.Domain.SeedWork;

    public class Configuration : Entity, IAggregateRoot
    {
        private readonly Dictionary<RuleCode, RuleConfiguration> rulesConfiguration = new Dictionary<RuleCode, RuleConfiguration>();

        public Configuration(
            long id,
            Dictionary<RuleCode, RuleConfiguration> rulesConfiguration,
            long? schemaGroupId,
            GroupConfiguration groupConfiguration)
            : base(id)
        {
            this.SchemaGroupId = schemaGroupId;
            this.SetGroupConfiguration(groupConfiguration);
            this.SetRulesConfiguration(rulesConfiguration);
        }

        public long? SchemaGroupId { get; }

        public GroupConfiguration GroupConfiguration { get; private set; }

        public IReadOnlyDictionary<RuleCode, RuleConfiguration> GetRulesConfiguration()
        {
            return new ReadOnlyDictionary<RuleCode, RuleConfiguration>(this.rulesConfiguration);
        }

        public IReadOnlyDictionary<RuleCode, RuleConfiguration> MergeRuleConfiguration(Configuration globalConfiguration)
        {
            if (globalConfiguration == null)
            {
                throw new ArgumentNullException($"{nameof(globalConfiguration)} cannot be null");
            }

            IReadOnlyDictionary<RuleCode, RuleConfiguration> globalRules = globalConfiguration.GetRulesConfiguration();

            var overriddenRules = this
                .GetRulesConfiguration()
                .Select(pair => pair.Value.Inherit
                    ? new KeyValuePair<RuleCode, RuleConfiguration>(pair.Key, globalRules[pair.Key])
                    : pair);

            return new Dictionary<RuleCode, RuleConfiguration>(overriddenRules);
        }

        public void SetRulesConfiguration(IReadOnlyDictionary<RuleCode, RuleConfiguration> configs)
        {
            if (configs == null)
            {
                throw new ArgumentNullException($"{nameof(configs)} cannot be null");
            }

            foreach (var configPair in configs)
            {
                var config = this.SchemaGroupId == default
                    ? configPair.Value.WithInheritance(false)
                    : configPair.Value;
                this.rulesConfiguration[configPair.Key] = config;
            }
        }

        public void SetGroupConfiguration(GroupConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException($"{nameof(configuration)} cannot be null");
            }

            this.GroupConfiguration = this.SchemaGroupId == null
                ? configuration.WithInheritance(false)
                : configuration;
        }
    }
}
