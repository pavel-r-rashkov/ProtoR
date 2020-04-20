namespace ProtoR.Domain.SchemaGroupAggregate.Schemas
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Google.Protobuf.Reflection;
    using static Google.Protobuf.Reflection.FieldDescriptorProto;
    using Type = Google.Protobuf.Reflection.FieldDescriptorProto.Type;

    public class ProtoBufSchema : Schema<FileDescriptorSet>
    {
        public ProtoBufSchema(long id, Version version, string contents)
            : base(id, version, contents)
        {
        }

        internal ProtoBufSchemaScope RootScope()
        {
            FileDescriptorProto fileDescriptor = this.Parsed.Files.First();

            return new ProtoBufSchemaScope(
                string.Empty,
                string.Empty,
                null,
                fileDescriptor.MessageTypes,
                Array.Empty<FieldDescriptorProto>(),
                fileDescriptor.EnumTypes,
                Array.Empty<OneofDescriptorProto>());
        }

        protected override FileDescriptorSet ParseContents()
        {
            var descriptorSet = new FileDescriptorSet();

            using (var contentsReader = new StringReader(this.Contents))
            {
                descriptorSet.Add(this.Id.ToString(CultureInfo.CurrentCulture), true, contentsReader);
                descriptorSet.Process();
                return descriptorSet;
            }
        }

        internal class ProtoBufSchemaScope
        {
            public static readonly FieldInfo OneOfIndexField = typeof(FieldDescriptorProto).GetField("__pbn__OneofIndex", BindingFlags.NonPublic | BindingFlags.Instance);

            public ProtoBufSchemaScope(
                string path,
                string name,
                DescriptorProto containingMessage,
                IEnumerable<DescriptorProto> messages,
                IEnumerable<FieldDescriptorProto> messageFields,
                IEnumerable<EnumDescriptorProto> enums,
                IEnumerable<OneofDescriptorProto> oneOfDefinitions)
            {
                this.Path = path;
                this.Name = name;
                this.ContainingMessage = containingMessage;
                this.Messages = messages;
                this.MessageFields = messageFields;
                this.Enums = enums;
                this.OneOfDefinitions = oneOfDefinitions;
            }

            public string Path { get; private set; }

            public string Name { get; private set; }

            public DescriptorProto ContainingMessage { get; private set; }

            public IEnumerable<DescriptorProto> Messages { get; private set; }

            public IEnumerable<FieldDescriptorProto> MessageFields { get; private set; }

            public IEnumerable<EnumDescriptorProto> Enums { get; private set; }

            public IEnumerable<OneofDescriptorProto> OneOfDefinitions { get; private set; }

            public static IEnumerable<T> ParallelTraverse<T>(
                ProtoBufSchemaScope a,
                ProtoBufSchemaScope b,
                Func<ProtoBufSchemaScope, ProtoBufSchemaScope, IList<T>> scopeVisitor)
            {
                List<ProtoBufSchemaScope> aChildScopes = a.GetChildScopes().ToList();
                List<ProtoBufSchemaScope> bChildScopes = b.GetChildScopes().ToList();
                var differences = new List<T>();

                differences.AddRange(scopeVisitor(a, b));

                var matchingScopes = aChildScopes.Join(
                    bChildScopes,
                    s => s.Name,
                    s => s.Name,
                    (ProtoBufSchemaScope aScope, ProtoBufSchemaScope bScope) => new { aScope, bScope });

                foreach (var matchingScope in matchingScopes)
                {
                    differences.AddRange(ParallelTraverse(matchingScope.aScope, matchingScope.bScope, scopeVisitor));
                }

                return differences;
            }

            public IEnumerable<ProtoBufSchemaScope> GetChildScopes()
            {
                foreach (var message in this.Messages.Where(m => m.Options == null || !m.Options.MapEntry))
                {
                    yield return new ProtoBufSchemaScope(
                        $"{this.Path}.{message.Name}",
                        message.Name,
                        message,
                        message.NestedTypes.Where(m => m.Options == null || !m.Options.MapEntry),
                        message.Fields,
                        message.EnumTypes,
                        message.OneofDecls);
                }
            }

            public DescriptorProto GetMapEntryMessage(FieldDescriptorProto field)
            {
                if (field.type != Type.TypeMessage || field.label != Label.LabelRepeated)
                {
                    return null;
                }

                var mapEntryName = new StringBuilder();
                var shouldChangeToUpperCase = true;

                foreach (var nameChar in field.Name.ToCharArray())
                {
                    if (nameChar == '_')
                    {
                        shouldChangeToUpperCase = true;
                        continue;
                    }

                    if (shouldChangeToUpperCase)
                    {
                        mapEntryName.Append(nameChar.ToString(CultureInfo.InvariantCulture).ToUpperInvariant());
                        shouldChangeToUpperCase = false;
                    }
                    else
                    {
                        mapEntryName.Append(nameChar);
                    }
                }

                mapEntryName.Append("Entry");

                return this.ContainingMessage.NestedTypes.FirstOrDefault(m =>
                    m.Name == mapEntryName.ToString()
                    && m.Options != null
                    && m.Options.MapEntry);
            }
        }
    }
}
