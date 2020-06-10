namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using static ProtoR.Domain.SchemaGroupAggregate.Schemas.ProtoBufSchema;

    public class FieldAddedRule : ProtoBufRule
    {
        public FieldAddedRule()
            : base(RuleCode.PB0008)
        {
        }

        public override ValidationResult Validate(ProtoBufSchema a, ProtoBufSchema b)
        {
            _ = a ?? throw new ArgumentNullException(nameof(a));
            _ = b ?? throw new ArgumentNullException(nameof(b));

            IEnumerable<string> addedFields = ProtoBufSchemaScope.ParallelTraverse(
                a.RootScope(),
                b.RootScope(),
                this.VisitScope);

            return addedFields.Any()
                ? new ValidationResult(this.Code, false, this.FormatAddedFields(addedFields))
                : new ValidationResult(this.Code, true, "No Fields were added");
        }

        private IList<string> VisitScope(ProtoBufSchemaScope a, ProtoBufSchemaScope b)
        {
            var addedFields = new List<string>();

            foreach (var messageField in a.MessageFields)
            {
                FieldDescriptorProto matchingMessageField = b.MessageFields.FirstOrDefault(e => e.Number == messageField.Number);

                if (matchingMessageField == null)
                {
                    addedFields.Add($"{a.Path}.{messageField.Number}");
                }
            }

            return addedFields;
        }

        private string FormatAddedFields(IEnumerable<string> addedFields)
        {
            return $"Added Fields:{Environment.NewLine}{string.Join(Environment.NewLine, addedFields)}";
        }
    }
}
