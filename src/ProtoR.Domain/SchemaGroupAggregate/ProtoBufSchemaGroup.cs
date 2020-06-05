namespace ProtoR.Domain.SchemaGroupAggregate
{
    using System.Collections.Generic;
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;

    public class ProtoBufSchemaGroup : SchemaGroup<ProtoBufSchema, FileDescriptorSet>
    {
        public ProtoBufSchemaGroup(
            string name,
            long categoryId)
            : base(
                name,
                categoryId,
                RuleFactory.GetProtoBufRules(),
                new ProtoBufSchemaFactory())
        {
        }

        public ProtoBufSchemaGroup(
            long id,
            string name,
            long categoryId,
            IEnumerable<ProtoBufSchema> schemas)
            : base(
                id,
                name,
                categoryId,
                schemas,
                RuleFactory.GetProtoBufRules(),
                new ProtoBufSchemaFactory())
        {
        }
    }
}
