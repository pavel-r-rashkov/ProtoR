namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;

    public class FieldRemovedRule : ProtoBufRule
    {
        public FieldRemovedRule()
            : base(RuleCode.PB0007)
        {
        }

        public override ValidationResult Validate(ProtoBufSchema a, ProtoBufSchema b)
        {
            a.Should().NotBeNull();
            b.Should().NotBeNull();

            IEnumerable<string> aFields = a.GetFieldNumbers();
            IEnumerable<string> bFields = b.GetFieldNumbers();

            IEnumerable<string> removedFields = bFields.Except(aFields);

            return removedFields.Any()
                ? new ValidationResult(false, this.FormatRemovedFields(removedFields))
                : new ValidationResult(true, "No Fields were removed");
        }

        private string FormatRemovedFields(IEnumerable<string> removedFields)
        {
            return $"Removed Fields:{Environment.NewLine}{string.Join(Environment.NewLine, removedFields)}";
        }
    }
}
