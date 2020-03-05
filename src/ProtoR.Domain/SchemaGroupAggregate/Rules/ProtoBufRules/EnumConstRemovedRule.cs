namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;

    public class EnumConstRemovedRule : ProtoBufRule
    {
        public EnumConstRemovedRule()
            : base(RuleCode.PB0009)
        {
        }

        public override ValidationResult Validate(ProtoBufSchema a, ProtoBufSchema b)
        {
            a.Should().NotBeNull();
            b.Should().NotBeNull();

            IEnumerable<string> aEnumConstants = a.GetEnumConstantNumbers();
            IEnumerable<string> bEnumConstants = b.GetEnumConstantNumbers();

            IEnumerable<string> removedEnumConstants = bEnumConstants.Except(aEnumConstants);

            return removedEnumConstants.Any()
                ? new ValidationResult(false, this.FormatRemovedEnumConstants(removedEnumConstants))
                : new ValidationResult(true, "No enum constants were removed");
        }

        private string FormatRemovedEnumConstants(IEnumerable<string> removedEnumConstants)
        {
            return $"Removed enum constants:{Environment.NewLine}{string.Join(Environment.NewLine, removedEnumConstants)}";
        }
    }
}
