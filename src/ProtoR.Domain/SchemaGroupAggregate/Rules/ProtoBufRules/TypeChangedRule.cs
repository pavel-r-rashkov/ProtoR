namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using static ProtoR.Domain.SchemaGroupAggregate.Schemas.ProtoBufSchema;
    using Type = Google.Protobuf.Reflection.FieldDescriptorProto.Type;

    public class TypeChangedRule : ProtoBufRule
    {
        public TypeChangedRule()
            : base(RuleCode.PB0019)
        {
        }

        protected TypeChangedRule(RuleCode code)
            : base(code)
        {
        }

        public override ValidationResult Validate(ProtoBufSchema a, ProtoBufSchema b)
        {
            _ = a ?? throw new ArgumentNullException(nameof(a));
            _ = b ?? throw new ArgumentNullException(nameof(b));

            IEnumerable<string> changedTypes = ProtoBufSchemaScope.ParallelTraverse(
                a.RootScope(),
                b.RootScope(),
                this.VisitScope);

            return changedTypes.Any()
                ? new ValidationResult(this.Code, false, this.FormatRemovedFields(changedTypes))
                : new ValidationResult(this.Code, true, "No types were changed");
        }

        private protected virtual bool CompareFieldTypes(
            FieldDescriptorProto aField,
            ProtoBufSchemaScope aParent,
            FieldDescriptorProto bField,
            ProtoBufSchemaScope bParent)
        {
            if (aField.type == bField.type
                && aField.type != Type.TypeMessage
                && aField.type != Type.TypeEnum)
            {
                return true;
            }

            if (aField.type == Type.TypeEnum && bField.type == Type.TypeEnum)
            {
                return aField.TypeName == bField.TypeName;
            }

            if (aField.type == Type.TypeMessage && bField.type == Type.TypeMessage)
            {
                return this.CompareMessageTypes(
                    aField,
                    aParent,
                    bField,
                    bParent);
            }

            return false;
        }

        private protected bool CompareMessageTypes(
            FieldDescriptorProto aField,
            ProtoBufSchemaScope aParent,
            FieldDescriptorProto bField,
            ProtoBufSchemaScope bParent)
        {
            DescriptorProto aMapEntryMessage = aParent.GetMapEntryMessage(aField);
            DescriptorProto bMapEntryMessage = bParent.GetMapEntryMessage(bField);

            if (aMapEntryMessage != null && bMapEntryMessage != null)
            {
                bool keyTypeCompatible = this.CompareFieldTypes(
                    aMapEntryMessage.Fields.First(f => f.Name == "key"),
                    null,
                    bMapEntryMessage.Fields.First(f => f.Name == "key"),
                    null);

                bool valueTypeCompatible = this.CompareFieldTypes(
                    aMapEntryMessage.Fields.First(f => f.Name == "value"),
                    null,
                    bMapEntryMessage.Fields.First(f => f.Name == "value"),
                    null);

                return keyTypeCompatible && valueTypeCompatible;
            }

            return aField.TypeName == bField.TypeName;
        }

        private IList<string> VisitScope(ProtoBufSchemaScope a, ProtoBufSchemaScope b)
        {
            var changedTypes = new List<string>();

            foreach (var messageField in a.MessageFields)
            {
                FieldDescriptorProto matchingMessageField = b.MessageFields.FirstOrDefault(e => e.Number == messageField.Number);

                if (matchingMessageField != null && !this.CompareFieldTypes(messageField, a, matchingMessageField, b))
                {
                    changedTypes.Add($"Type of field {a.Path}.{messageField.Number} was changed");
                }
            }

            return changedTypes;
        }

        private string FormatRemovedFields(IEnumerable<string> changedTypes)
        {
            return $"Changed types:{Environment.NewLine}{string.Join(Environment.NewLine, changedTypes)}";
        }
    }
}
