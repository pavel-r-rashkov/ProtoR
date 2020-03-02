namespace ProtoR.Domain.SchemaGroupAggregate.Schemas
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Google.Protobuf.Reflection;

    public class ProtoBufSchema : Schema<FileDescriptorSet>
    {
        public ProtoBufSchema(Guid id, Version version, string contents)
            : base(id, version, contents)
        {
        }

        public IEnumerable<string> GetMessageTypeNames()
        {
            return this.Parsed.Files.SelectMany(f =>
                f.MessageTypes.SelectMany(messageType => this.GetMessageTypeNames(messageType)));
        }

        public IEnumerable<string> GetEnumTypeNames()
        {
            IEnumerable<string> outerScopeEnums = this.Parsed.Files
                .SelectMany(f => f.EnumTypes)
                .Select(e => FormatName(e.Name));

            IEnumerable<string> innerScopeEnums = this.Parsed.Files.SelectMany(f =>
                f.MessageTypes.SelectMany(messageType => this.GetEnumTypeNames(messageType)));

            return outerScopeEnums.Union(innerScopeEnums);
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

        private static string FormatName(string name)
        {
            return $".{name}";
        }

        private IEnumerable<string> GetMessageTypeNames(DescriptorProto message, string parentName = "")
        {
            var messageName = $"{parentName}{FormatName(message.Name)}";
            var messages = new List<string>
            {
                messageName,
            };

            IEnumerable<string> nestedMessageNames = message.NestedTypes
                .SelectMany(messageType => this.GetMessageTypeNames(messageType, messageName));

            return messages.Union(nestedMessageNames);
        }

        private IEnumerable<string> GetEnumTypeNames(DescriptorProto message, string parentName = "")
        {
            var messageName = $"{parentName}{FormatName(message.Name)}";
            List<string> enums = message.EnumTypes
                .Select(e => $"{messageName}{FormatName(e.Name)}")
                .ToList();

            IEnumerable<string> nestedEnumNames = message.NestedTypes
                .SelectMany(messageType => this.GetEnumTypeNames(messageType, messageName));

            return enums.Union(nestedEnumNames);
        }
    }
}
