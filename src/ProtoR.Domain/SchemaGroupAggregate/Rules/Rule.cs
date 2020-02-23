namespace ProtoR.Domain.SchemaGroupAggregate.Rules
{
    using System.Collections.Generic;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.SeedWork;

    public abstract class Rule<TSchemaContents> : ValueObject<Rule<TSchemaContents>>
    {
        public Rule(RuleCode code)
        {
            this.Code = code;
        }

        public RuleCode Code { get; }

        public abstract ValidationResult Validate(Schema<TSchemaContents> a, Schema<TSchemaContents> b);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Code;
        }
    }
}
