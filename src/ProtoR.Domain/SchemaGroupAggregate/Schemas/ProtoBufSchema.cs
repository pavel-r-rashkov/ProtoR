namespace ProtoR.Domain.SchemaGroupAggregate.Schemas
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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
            return this.GetTypeNames(
                (FileDescriptorProto fileDescriptor) => fileDescriptor.MessageTypes.Select(e => e.Name),
                (DescriptorProto descriptorProto) => new string[] { descriptorProto.Name });
        }

        public IEnumerable<string> GetEnumTypeNames()
        {
            return this.GetTypeNames(
                (FileDescriptorProto fileDescriptor) => fileDescriptor.EnumTypes.Select(e => e.Name),
                (DescriptorProto descriptorProto) => descriptorProto.EnumTypes.Select(e => e.Name));
        }

        public IEnumerable<string> GetOneOfTypeNames()
        {
            return this.GetTypeNames(
                (FileDescriptorProto fileDescriptor) => Array.Empty<string>(),
                (DescriptorProto descriptorProto) => descriptorProto.OneofDecls.Select(e => e.Name));
        }

        public IEnumerable<string> GetFieldTypeNumbers()
        {
            return this.GetTypeNames(
                (FileDescriptorProto fileDescriptor) => Array.Empty<string>(),
                (DescriptorProto descriptorProto) => descriptorProto.Fields.Select(e => e.Number.ToString(CultureInfo.InvariantCulture)));
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

        private IEnumerable<string> GetTypeNames(
            Func<FileDescriptorProto, IEnumerable<string>> fromFileDescriptor,
            Func<DescriptorProto, IEnumerable<string>> fromDescriptorProto)
        {
            IEnumerable<string> outerScopeEnums = this.Parsed.Files
                .SelectMany(f => fromFileDescriptor(f))
                .Select(name => FormatName(name));

            IEnumerable<string> innerScopeEnums = this.Parsed.Files.SelectMany(f =>
                f.MessageTypes.SelectMany(messageType => this.GetTypeNames(fromDescriptorProto, messageType)));

            return outerScopeEnums.Union(innerScopeEnums);
        }

        private IEnumerable<string> GetTypeNames(
            Func<DescriptorProto, IEnumerable<string>> fromDescriptorProto,
            DescriptorProto message,
            string parentName = "")
        {
            var messageName = $"{parentName}{FormatName(message.Name)}";
            List<string> enums = fromDescriptorProto(message)
                .Select(name => $"{messageName}{FormatName(name)}")
                .ToList();

            IEnumerable<string> nestedEnumNames = message.NestedTypes
                .SelectMany(messageType => this.GetTypeNames(fromDescriptorProto, messageType, messageName));

            return enums.Union(nestedEnumNames);
        }
    }
}
