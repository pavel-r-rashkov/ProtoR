namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;

    public class OneOfAddedRule : ProtoBufRule
    {
        public OneOfAddedRule()
            : base(RuleCode.PB0006)
        {
        }

        public override ValidationResult Validate(ProtoBufSchema a, ProtoBufSchema b)
        {
            a.Should().NotBeNull();
            b.Should().NotBeNull();

            IEnumerable<string> aTypes = a.GetOneOfTypeNames();
            IEnumerable<string> bTypes = b.GetOneOfTypeNames();

            IEnumerable<string> removedOneOfTypes = bTypes.Except(aTypes);

            return removedOneOfTypes.Any()
                ? new ValidationResult(false, this.FormatRemovedOneOfTypes(removedOneOfTypes))
                : new ValidationResult(true, "No OneOf types were added");
        }

        private string FormatRemovedOneOfTypes(IEnumerable<string> removedOneOfTypes)
        {
            return $"Added OneOf types:{Environment.NewLine}{string.Join(Environment.NewLine, removedOneOfTypes)}";
        }
    }
}
