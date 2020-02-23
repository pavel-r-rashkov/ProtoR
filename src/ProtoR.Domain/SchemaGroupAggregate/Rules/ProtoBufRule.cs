namespace ProtoR.Domain.SchemaGroupAggregate.Rules
{
    using Google.Protobuf.Reflection;

    public abstract class ProtoBufRule : Rule<FileDescriptorSet>
    {
        public ProtoBufRule(RuleCode code)
            : base(code)
        {
        }
    }
}
