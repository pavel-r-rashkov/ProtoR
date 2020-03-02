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
            Func<FileDescriptorProto, IEnumerable<string>> fromFileDescriptor =
                (FileDescriptorProto fileDescriptor) => fileDescriptor.MessageTypes.Select(e => e.Name);
            Func<DescriptorProto, IEnumerable<string>> fromDescriptorProto =
                (DescriptorProto descriptorProto) => new string[] { descriptorProto.Name };

            return this.GetTypeNames(fromFileDescriptor, fromDescriptorProto);
        }

        public IEnumerable<string> GetEnumTypeNames()
        {
            Func<FileDescriptorProto, IEnumerable<string>> fromFileDescriptor =
                (FileDescriptorProto fileDescriptor) => fileDescriptor.EnumTypes.Select(e => e.Name);
            Func<DescriptorProto, IEnumerable<string>> fromDescriptorProto =
                (DescriptorProto descriptorProto) => descriptorProto.EnumTypes.Select(e => e.Name);

            return this.GetTypeNames(fromFileDescriptor, fromDescriptorProto);
        }

        public IEnumerable<string> GetOneOfTypeNames()
        {
            Func<FileDescriptorProto, IEnumerable<string>> fromFileDescriptor =
                (FileDescriptorProto fileDescriptor) => Array.Empty<string>();
            Func<DescriptorProto, IEnumerable<string>> fromDescriptorProto =
                (DescriptorProto descriptorProto) => descriptorProto.OneofDecls.Select(e => e.Name);

            return this.GetTypeNames(fromFileDescriptor, fromDescriptorProto);
        }

        public IEnumerable<string> GetFieldTypeNumbers()
        {
            Func<FileDescriptorProto, IEnumerable<string>> fromFileDescriptor =
                (FileDescriptorProto fileDescriptor) => Array.Empty<string>();
            Func<DescriptorProto, IEnumerable<string>> fromDescriptorProto =
                (DescriptorProto descriptorProto) => descriptorProto.Fields.Select(e => e.Number.ToString(CultureInfo.InvariantCulture));

            return this.GetTypeNames(fromFileDescriptor, fromDescriptorProto);
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
