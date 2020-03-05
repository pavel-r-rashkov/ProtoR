namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;

    public class FieldAddedRule : ProtoBufRule
    {
        public FieldAddedRule()
            : base(RuleCode.PB0008)
        {
        }

        public override ValidationResult Validate(ProtoBufSchema a, ProtoBufSchema b)
        {
            a.Should().NotBeNull();
            b.Should().NotBeNull();

            IEnumerable<string> aTypes = a.GetFieldNumbers();
            IEnumerable<string> bTypes = b.GetFieldNumbers();

            IEnumerable<string> addedFields = aTypes.Except(bTypes);

            return addedFields.Any()
                ? new ValidationResult(false, this.FormatAddedFields(addedFields))
                : new ValidationResult(true, "No Fields were added");
        }

        private string FormatAddedFields(IEnumerable<string> addedFields)
        {
            return $"Added Fields:{Environment.NewLine}{string.Join(Environment.NewLine, addedFields)}";
        }
    }
}
