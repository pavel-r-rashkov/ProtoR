namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;

    public class EnumRemovedRule : ProtoBufRule
    {
        public EnumRemovedRule()
            : base(RuleCode.PB0003)
        {
        }

        public override ValidationResult Validate(ProtoBufSchema a, ProtoBufSchema b)
        {
            a.Should().NotBeNull();
            b.Should().NotBeNull();

            IEnumerable<string> aTypes = a.GetEnumTypeNames();
            IEnumerable<string> bTypes = b.GetEnumTypeNames();

            IEnumerable<string> removedEnumTypes = aTypes.Except(bTypes);

            return removedEnumTypes.Any()
                ? new ValidationResult(false, this.FormatRemovedEnumTypes(removedEnumTypes))
                : new ValidationResult(true, "No Enum types were removed");
        }

        private string FormatRemovedEnumTypes(IEnumerable<string> removedEnumTypes)
        {
            return $"Removed Enum types:{Environment.NewLine}{string.Join(Environment.NewLine, removedEnumTypes)}";
        }
    }
}
