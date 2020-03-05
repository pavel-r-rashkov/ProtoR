namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;

    public class FieldRenamedRule : ProtoBufRule
    {
        public FieldRenamedRule()
            : base(RuleCode.PB0013)
        {
        }

        public override ValidationResult Validate(ProtoBufSchema a, ProtoBufSchema b)
        {
            a.Should().NotBeNull();
            b.Should().NotBeNull();

            IEnumerable<Field> aFields = this.GetFields(a);
            IEnumerable<Field> bFields = this.GetFields(b);

            var renamedFields = new List<Field>();

            foreach (var field in aFields)
            {
                Field matchingField = bFields.FirstOrDefault(f => f.FullyQualifiedName == field.FullyQualifiedName);

                if (matchingField != null && matchingField.Name != field.Name)
                {
                    field.RenamedFrom = matchingField.Name;
                    renamedFields.Add(field);
                }
            }

            return renamedFields.Any()
                ? new ValidationResult(false, $"Fields were renamed:{Environment.NewLine}{this.FormatRenamedFields(renamedFields)}")
                : new ValidationResult(true, "No fields were renamed.");
        }

        private string FormatRenamedFields(List<Field> renamedFields)
        {
            var messages = renamedFields.Select(field => $"{field.FullyQualifiedName} was renamed from {field.RenamedFrom} to {field.Name}");
            return string.Join(Environment.NewLine, messages);
        }

        private IEnumerable<Field> GetFields(ProtoBufSchema schema)
        {
            return schema.Parsed.Files
                .SelectMany(f => f.MessageTypes.SelectMany(message => this.GetFields(message)));
        }

        private IEnumerable<Field> GetFields(DescriptorProto message, string parentName = "")
        {
            var messageName = $"{parentName}.{message.Name}";

            IEnumerable<Field> fields = message.Fields.Select(field => new Field
            {
                FullyQualifiedName = $"{messageName}.{field.Number}",
                Name = field.Name,
            });

            IEnumerable<Field> nestedFields = message.NestedTypes.SelectMany(nestedMessage => this.GetFields(nestedMessage, messageName));

            return fields.Union(nestedFields);
        }

        private class Field
        {
            public string FullyQualifiedName { get; set; }

            public string Name { get; set; }

            public string RenamedFrom { get; set; }
        }
    }
}
