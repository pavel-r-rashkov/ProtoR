namespace ProtoR.Domain.SchemaGroupAggregate.Schemas
{
    using System;
    using System.IO;
    using Google.Protobuf.Reflection;

    public class ProtoBufSchema : Schema<FileDescriptorSet>
    {
        public ProtoBufSchema(Guid id, Version version, string contents)
            : base(id, version, contents)
        {
        }

        protected override FileDescriptorSet ParseContents()
        {
            var descriptorSet = new FileDescriptorSet();

            using (var contentsReader = new StringReader(this.Contents))
            {
                descriptorSet.Add(this.Id.ToString(), true, contentsReader);
                descriptorSet.Process();
                return descriptorSet;
            }
        }
    }
}
