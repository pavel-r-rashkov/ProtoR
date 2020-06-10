namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using static ProtoR.Domain.SchemaGroupAggregate.Schemas.ProtoBufSchema;

    public class MessageAddedRule : ProtoBufRule
    {
        public MessageAddedRule()
            : base(RuleCode.PB0002)
        {
        }

        public override ValidationResult Validate(ProtoBufSchema a, ProtoBufSchema b)
        {
            _ = a ?? throw new ArgumentNullException(nameof(a));
            _ = b ?? throw new ArgumentNullException(nameof(b));

            IEnumerable<string> addedMessageTypes = ProtoBufSchemaScope.ParallelTraverse(
                a.RootScope(),
                b.RootScope(),
                this.VisitScope);

            return addedMessageTypes.Any()
                ? new ValidationResult(this.Code, false, this.FormatRemovedMessageTypes(addedMessageTypes))
                : new ValidationResult(this.Code, true, "No message types were added");
        }

        private IList<string> VisitScope(ProtoBufSchemaScope a, ProtoBufSchemaScope b)
        {
            var addedMessages = new List<string>();

            foreach (var message in a.Messages)
            {
                DescriptorProto matchingMessage = b.Messages.FirstOrDefault(m => m.Name == message.Name);

                if (matchingMessage == null)
                {
                    addedMessages.Add($"{a.Path}.{message.Name}");
                }
            }

            return addedMessages;
        }

        private string FormatRemovedMessageTypes(IEnumerable<string> addedMessageTypes)
        {
            return $"Added message types:{Environment.NewLine}{string.Join(Environment.NewLine, addedMessageTypes)}";
        }
    }
}
