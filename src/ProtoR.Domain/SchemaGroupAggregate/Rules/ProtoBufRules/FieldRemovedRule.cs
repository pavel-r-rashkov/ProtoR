namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
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
            a.Should().NotBeNull();
            b.Should().NotBeNull();

            IEnumerable<string> removedFields = ProtoBufSchemaScope.ParallelTraverse(
                a.RootScope(),
                b.RootScope(),
                this.VisitScope);

            return removedFields.Any()
                ? new ValidationResult(false, this.FormatRemovedFields(removedFields))
                : new ValidationResult(true, "No Fields were removed");
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
