namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;

    public class EnumAddedRule : ProtoBufRule
    {
        public EnumAddedRule()
            : base(RuleCode.PB0004)
        {
        }

        public override ValidationResult Validate(ProtoBufSchema a, ProtoBufSchema b)
        {
            a.Should().NotBeNull();
            b.Should().NotBeNull();

            IEnumerable<string> aTypes = a.GetEnumTypeNames();
            IEnumerable<string> bTypes = b.GetEnumTypeNames();

            IEnumerable<string> addedEnumTypes = aTypes.Except(bTypes);

            return addedEnumTypes.Any()
                ? new ValidationResult(false, this.FormatRemovedEnumTypes(addedEnumTypes))
                : new ValidationResult(true, "No enum types were added");
        }

        private string FormatRemovedEnumTypes(IEnumerable<string> addedEnumTypes)
        {
            return $"Added enum types:{Environment.NewLine}{string.Join(Environment.NewLine, addedEnumTypes)}";
        }
    }
}
