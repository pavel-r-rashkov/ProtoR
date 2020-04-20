namespace ProtoR.Domain.SchemaGroupAggregate.Schemas
{
    using System.IO;
    using Google.Protobuf.Reflection;
    using ProtoBuf.Reflection;

    public class ProtoBufSchemaFactory : ISchemaFactory<ProtoBufSchema, FileDescriptorSet>
    {
        public ProtoBufSchema CreateNew(Version version, string contents)
        {
            this.Validate(contents);

            return new ProtoBufSchema(
                default,
                version,
                contents);
        }

        private void Validate(string contents)
        {
            var descriptorSet = new FileDescriptorSet();

            using (var contentsReader = new StringReader(contents))
            {
                descriptorSet.Add("protobufSchema", true, contentsReader);
                descriptorSet.Process();
                Error[] errors = descriptorSet.GetErrors();

                if (errors.Length > 0)
                {
                    throw new InvalidProtoBufSchemaException(errors);
                }
            }
        }
    }
}
