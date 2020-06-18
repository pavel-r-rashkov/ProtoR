namespace ProtoR.Domain.SchemaGroupAggregate
{
    using System.Collections.Generic;
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;

    public class ProtoBufSchemaGroup : SchemaGroup<ProtoBufSchema, FileDescriptorSet>
    {
        public ProtoBufSchemaGroup(
            string name)
            : base(
                name,
                RuleFactory.GetProtoBufRules(),
                new ProtoBufSchemaFactory())
        {
        }

        public ProtoBufSchemaGroup(
            long id,
            string name,
            IEnumerable<ProtoBufSchema> schemas)
            : base(
                id,
                name,
                schemas,
                RuleFactory.GetProtoBufRules(),
                new ProtoBufSchemaFactory())
        {
        }
    }
}
