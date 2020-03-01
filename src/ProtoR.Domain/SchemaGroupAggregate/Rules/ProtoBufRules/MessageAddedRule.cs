namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;

    public class MessageAddedRule : ProtoBufRule
    {
        public MessageAddedRule()
            : base(RuleCode.PB0002)
        {
        }

        public override ValidationResult Validate(ProtoBufSchema a, ProtoBufSchema b)
        {
            a.Should().NotBeNull();
            b.Should().NotBeNull();

            IEnumerable<string> aTypes = a.GetMessageTypeNames();
            IEnumerable<string> bTypes = b.GetMessageTypeNames();

            IEnumerable<string> removedMessageTypes = bTypes.Except(aTypes);

            return removedMessageTypes.Any()
                ? new ValidationResult(false, this.FormatRemovedMessageTypes(removedMessageTypes))
                : new ValidationResult(true, "No message types were added");
        }

        private string FormatRemovedMessageTypes(IEnumerable<string> removedMessageTypes)
        {
            return $"Added message types:{Environment.NewLine}{string.Join(Environment.NewLine, removedMessageTypes)}";
        }
    }
}
