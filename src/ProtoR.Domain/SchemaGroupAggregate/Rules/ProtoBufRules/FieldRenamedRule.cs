namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using static ProtoR.Domain.SchemaGroupAggregate.Schemas.ProtoBufSchema;

    public class FieldRenamedRule : ProtoBufRule
    {
        public FieldRenamedRule()
            : base(RuleCode.PB0013)
        {
        }

        public override ValidationResult Validate(ProtoBufSchema a, ProtoBufSchema b)
        {
            _ = a ?? throw new ArgumentNullException(nameof(a));
            _ = b ?? throw new ArgumentNullException(nameof(b));

            IEnumerable<Field> renamedFields = ProtoBufSchemaScope.ParallelTraverse(
                a.RootScope(),
                b.RootScope(),
                this.VisitScope);

            return renamedFields.Any()
                ? new ValidationResult(this.Code, false, $"Fields were renamed:{Environment.NewLine}{this.FormatRenamedFields(renamedFields)}")
                : new ValidationResult(this.Code, true, "No fields were renamed.");
        }

        private IList<Field> VisitScope(ProtoBufSchemaScope a, ProtoBufSchemaScope b)
        {
            var addedFields = new List<Field>();

            foreach (var messageField in a.MessageFields)
            {
                FieldDescriptorProto matchingMessageField = b.MessageFields.FirstOrDefault(e => e.Number == messageField.Number);

                if (matchingMessageField != null && messageField.Name != matchingMessageField.Name)
                {
                    addedFields.Add(new Field
                    {
                        FullyQualifiedName = $"{a.Path}.{messageField.Number}",
                        Name = messageField.Name,
                        RenamedFrom = matchingMessageField.Name,
                    });
                }
            }

            return addedFields;
        }

        private string FormatRenamedFields(IEnumerable<Field> renamedFields)
        {
            var messages = renamedFields.Select(field => $"{field.FullyQualifiedName} was renamed from {field.RenamedFrom} to {field.Name}");
            return string.Join(Environment.NewLine, messages);
        }

        private class Field
        {
            public string FullyQualifiedName { get; set; }

            public string Name { get; set; }

            public string RenamedFrom { get; set; }
        }
    }
}
