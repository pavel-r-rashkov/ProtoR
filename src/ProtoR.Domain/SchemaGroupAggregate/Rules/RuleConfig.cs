namespace ProtoR.Domain.SchemaGroupAggregate.Rules
{
    using System.Collections.Generic;
    using ProtoR.Domain.SeedWork;

    public class RuleConfig : ValueObject<RuleConfig>
    {
        public RuleConfig(bool shouldInherit, Severity severity)
        {
            this.ShouldInherit = shouldInherit;
            this.Severity = severity;
        }

        public bool ShouldInherit { get; }

        public Severity Severity { get; }

        public RuleConfig WithInheritance(bool shouldInherit)
        {
            return new RuleConfig(shouldInherit, this.Severity);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.ShouldInherit;
            yield return this.Severity;
        }
    }
}
