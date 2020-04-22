namespace ProtoR.Domain.ConfigurationAggregate
{
    using System.Collections.Generic;
    using ProtoR.Domain.SeedWork;

    public class RuleConfiguration : ValueObject<RuleConfiguration>
    {
        public RuleConfiguration(bool inherit, Severity severity)
        {
            this.Inherit = inherit;
            this.Severity = severity;
        }

        public bool Inherit { get; }

        public Severity Severity { get; }

        public RuleConfiguration WithInheritance(bool inherit)
        {
            return new RuleConfiguration(inherit, this.Severity);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Inherit;
            yield return this.Severity;
        }
    }
}
