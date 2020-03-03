namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;

    public class EnumConstAddedRule : ProtoBufRule
    {
        public EnumConstAddedRule()
            : base(RuleCode.PB0010)
        {
        }

        public override ValidationResult Validate(ProtoBufSchema a, ProtoBufSchema b)
        {
            a.Should().NotBeNull();
            b.Should().NotBeNull();

            IEnumerable<string> aEnumConstants = a.GetEnumConstantNumbers();
            IEnumerable<string> bEnumConstants = b.GetEnumConstantNumbers();

            IEnumerable<string> addedEnumConstants = bEnumConstants.Except(aEnumConstants);

            return addedEnumConstants.Any()
                ? new ValidationResult(false, this.FormatAddedEnumConstants(addedEnumConstants))
                : new ValidationResult(true, "No enum constants were added");
        }

        private string FormatAddedEnumConstants(IEnumerable<string> addedEnumConstants)
        {
            return $"Added enum constants:{Environment.NewLine}{string.Join(Environment.NewLine, addedEnumConstants)}";
        }
    }
}
