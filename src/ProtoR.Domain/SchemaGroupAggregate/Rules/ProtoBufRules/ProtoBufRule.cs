namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;

    public abstract class ProtoBufRule : Rule<ProtoBufSchema, FileDescriptorSet>
    {
        public ProtoBufRule(RuleCode code)
            : base(code)
        {
        }
    }
}
