namespace ProtoR.Domain.SchemaGroupAggregate.Rules
{
    using System.Collections.Generic;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.SeedWork;

    public abstract class Rule<TSchema, TSchemaContents> : ValueObject<Rule<TSchema, TSchemaContents>>
        where TSchema : Schema<TSchemaContents>
    {
        public Rule(RuleCode code)
        {
            this.Code = code;
        }

        public RuleCode Code { get; }

        public abstract ValidationResult Validate(TSchema a, TSchema b);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Code;
        }
    }
}
