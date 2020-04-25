namespace ProtoR.Domain.SchemaGroupAggregate
{
    using System.Collections.Generic;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using ProtoR.Domain.SeedWork;

    public class ValidationResult : ValueObject<ValidationResult>
    {
        public ValidationResult(RuleCode ruleCode, bool passed, string description)
        {
            this.RuleCode = ruleCode;
            this.Passed = passed;
            this.Description = description;
        }

        public RuleCode RuleCode { get; }

        public bool Passed { get; }

        public string Description { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Passed;
            yield return this.Description;
        }
    }
}
