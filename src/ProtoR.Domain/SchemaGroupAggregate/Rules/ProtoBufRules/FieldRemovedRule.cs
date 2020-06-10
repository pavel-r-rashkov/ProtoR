namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using static ProtoR.Domain.SchemaGroupAggregate.Schemas.ProtoBufSchema;

    public class FieldRemovedRule : ProtoBufRule
    {
        public FieldRemovedRule()
            : base(RuleCode.PB0007)
        {
        }

        public override ValidationResult Validate(ProtoBufSchema a, ProtoBufSchema b)
        {
            _ = a ?? throw new ArgumentNullException(nameof(a));
            _ = b ?? throw new ArgumentNullException(nameof(b));

            IEnumerable<string> removedFields = ProtoBufSchemaScope.ParallelTraverse(
                a.RootScope(),
                b.RootScope(),
                this.VisitScope);

            return removedFields.Any()
                ? new ValidationResult(this.Code, false, this.FormatRemovedFields(removedFields))
                : new ValidationResult(this.Code, true, "No Fields were removed");
        }

        private IList<string> VisitScope(ProtoBufSchemaScope a, ProtoBufSchemaScope b)
        {
            var removedFields = new List<string>();

            foreach (var messageField in b.MessageFields)
            {
                FieldDescriptorProto matchingMessageField = a.MessageFields.FirstOrDefault(e => e.Number == messageField.Number);

                if (matchingMessageField == null)
                {
                    removedFields.Add($"{b.Path}.{messageField.Number}");
                }
            }

            return removedFields;
        }

        private string FormatRemovedFields(IEnumerable<string> removedFields)
        {
            return $"Removed Fields:{Environment.NewLine}{string.Join(Environment.NewLine, removedFields)}";
        }
    }
}
