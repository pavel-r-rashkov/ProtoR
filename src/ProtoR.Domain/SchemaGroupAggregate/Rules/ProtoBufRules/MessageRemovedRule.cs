namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using static ProtoR.Domain.SchemaGroupAggregate.Schemas.ProtoBufSchema;

    public class MessageRemovedRule : ProtoBufRule
    {
        public MessageRemovedRule()
            : base(RuleCode.PB0001)
        {
        }

        public override ValidationResult Validate(ProtoBufSchema a, ProtoBufSchema b)
        {
            a.Should().NotBeNull();
            b.Should().NotBeNull();

            IEnumerable<string> removedMessageTypes = ProtoBufSchemaScope.ParallelTraverse(
                a.RootScope(),
                b.RootScope(),
                this.VisitScope);

            return removedMessageTypes.Any()
                ? new ValidationResult(this.Code, false, this.FormatRemovedMessageTypes(removedMessageTypes))
                : new ValidationResult(this.Code, true, "No message types were removed");
        }

        private IList<string> VisitScope(ProtoBufSchemaScope a, ProtoBufSchemaScope b)
        {
            var removedMessages = new List<string>();

            foreach (var message in b.Messages)
            {
                DescriptorProto matchingMessage = a.Messages.FirstOrDefault(m => m.Name == message.Name);

                if (matchingMessage == null)
                {
                    removedMessages.Add($"{b.Path}.{message.Name}");
                }
            }

            return removedMessages;
        }

        private string FormatRemovedMessageTypes(IEnumerable<string> removedMessageTypes)
        {
            return $"Removed message types:{Environment.NewLine}{string.Join(Environment.NewLine, removedMessageTypes)}";
        }
    }
}
