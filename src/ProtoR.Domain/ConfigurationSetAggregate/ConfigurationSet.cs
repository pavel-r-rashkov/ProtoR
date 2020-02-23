namespace ProtoR.Domain.GlobalConfigurationAggregate
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using ProtoR.Domain.SeedWork;

    public class ConfigurationSet : Entity, IAggregateRoot
    {
        private Dictionary<RuleCode, RuleConfig> rulesConfig = new Dictionary<RuleCode, RuleConfig>();
        private bool shouldInherit;

        public ConfigurationSet(
            Guid id,
            Dictionary<RuleCode, RuleConfig> rulesConfig,
            Guid schemaGroupId,
            bool shouldInherit,
            bool forwardCompatible,
            bool backwardCompatible,
            bool transitive)
            : base(id)
        {
            this.shouldInherit = shouldInherit;
            this.SetRulesConfiguration(rulesConfig);
            this.SchemaGroupId = schemaGroupId;
            this.SetCompatibility(backwardCompatible, forwardCompatible);
            this.Transitive = transitive;
        }

        public Guid? SchemaGroupId { get; }

        public bool ShouldInherit
        {
            get
            {
                return this.shouldInherit;
            }

            set
            {
                if (this.SchemaGroupId != default)
                {
                    this.shouldInherit = value;
                }
            }
        }

        public bool ForwardCompatible { get; private set; }

        public bool BackwardCompatible { get; private set; }

        public bool Transitive { get; }

        public IReadOnlyDictionary<RuleCode, RuleConfig> GetRulesConfiguration()
        {
            return new ReadOnlyDictionary<RuleCode, RuleConfig>(this.rulesConfig);
        }

        public void SetRulesConfiguration(IReadOnlyDictionary<RuleCode, RuleConfig> configs)
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
                this.rulesConfig[configPair.Key] = config;
            }
        }

        public void SetCompatibility(bool backwardCompatible, bool forwardCompatible)
        {
            if (!backwardCompatible && !forwardCompatible)
            {
                throw new ArgumentException($"{nameof(backwardCompatible)} and {nameof(forwardCompatible)} cannot be both false");
            }

            this.ForwardCompatible = forwardCompatible;
            this.BackwardCompatible = backwardCompatible;
        }
    }
}
