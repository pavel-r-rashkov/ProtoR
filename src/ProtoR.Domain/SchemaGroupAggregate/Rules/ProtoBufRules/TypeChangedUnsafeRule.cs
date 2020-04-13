namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System.Linq;
    using Google.Protobuf.Reflection;
    using static Google.Protobuf.Reflection.FieldDescriptorProto;
    using static ProtoR.Domain.SchemaGroupAggregate.Schemas.ProtoBufSchema;

    public class TypeChangedUnsafeRule : TypeChangedRule
    {
        private readonly Type[][] compatibilityGroups = new[]
        {
            new[] { Type.TypeInt32, Type.TypeInt64, Type.TypeUint32, Type.TypeUint64, Type.TypeBool, Type.TypeEnum },
            new[] { Type.TypeSint32, Type.TypeSint64 },
            new[] { Type.TypeString, Type.TypeBytes },
            new[] { Type.TypeFixed32, Type.TypeSfixed32 },
            new[] { Type.TypeFixed64, Type.TypeSfixed64 },
            new[] { Type.TypeFloat },
            new[] { Type.TypeDouble },
        };

        public TypeChangedUnsafeRule()
            : base(RuleCode.PB0020)
        {
        }

        private protected override bool CompareFieldTypes(
            FieldDescriptorProto aField,
            ProtoBufSchemaScope aParent,
            FieldDescriptorProto bField,
            ProtoBufSchemaScope bParent)
        {
            if (aField.type == Type.TypeMessage && bField.type == Type.TypeMessage)
            {
                return this.CompareMessageTypes(
                    aField,
                    aParent,
                    bField,
                    bParent);
            }

            if (aField.type != Type.TypeMessage && bField.type != Type.TypeMessage)
            {
                Type[] group = this.compatibilityGroups.FirstOrDefault(g => g.Contains(aField.type));

                return group != null && group.Contains(bField.type);
            }

            return false;
        }
    }
}
