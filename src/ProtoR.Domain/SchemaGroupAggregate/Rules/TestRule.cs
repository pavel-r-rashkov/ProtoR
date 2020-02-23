namespace ProtoR.Domain.SchemaGroupAggregate.Rules
{
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;

    public class TestRule : ProtoBufRule
    {
        public TestRule()
            : base(RuleCode.R0001)
        {
        }

        public override ValidationResult Validate(Schema<FileDescriptorSet> a, Schema<FileDescriptorSet> b)
        {
            return new ValidationResult(true, "sample description");
        }
    }
}
