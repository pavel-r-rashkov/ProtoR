namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using static ProtoR.Domain.SchemaGroupAggregate.Schemas.ProtoBufSchema;

    public class MissingFieldReservationRule : ProtoBufRule
    {
        public MissingFieldReservationRule()
            : base(RuleCode.PB0015)
        {
        }

        public override ValidationResult Validate(ProtoBufSchema a, ProtoBufSchema b)
        {
            _ = a ?? throw new ArgumentNullException(nameof(a));
            _ = b ?? throw new ArgumentNullException(nameof(b));

            IEnumerable<FieldReservation> missingReservations = ProtoBufSchemaScope.ParallelTraverse(
                a.RootScope(),
                b.RootScope(),
                this.VisitScope);

            return missingReservations.Any()
                ? new ValidationResult(this.Code, false, this.FormatMissingReservations(missingReservations))
                : new ValidationResult(this.Code, true, "No field reservations are missing.");
        }

        private IList<FieldReservation> VisitScope(ProtoBufSchemaScope a, ProtoBufSchemaScope b)
        {
            var missingReservations = new List<FieldReservation>();

            foreach (var messageField in b.MessageFields)
            {
                FieldDescriptorProto matchingMessageField = a.MessageFields.FirstOrDefault(e => e.Number == messageField.Number);

                if (matchingMessageField == null && a.ContainingMessage != null)
                {
                    bool nameReserved = a.ContainingMessage.ReservedNames.Contains(messageField.Name);
                    bool numberReserved = a.ContainingMessage.ReservedRanges.Any(range =>
                        range.Start <= messageField.Number
                        && messageField.Number < range.End);

                    if (!nameReserved || !numberReserved)
                    {
                        missingReservations.Add(new FieldReservation
                        {
                            NameReserved = nameReserved,
                            NumberReserved = numberReserved,
                            FullyQualifiedName = $"{b.Path}.{messageField.Number}",
                            FieldName = messageField.Name,
                            FieldNumber = messageField.Number,
                        });
                    }
                }
            }

            return missingReservations;
        }

        private string FormatMissingReservations(IEnumerable<FieldReservation> reseravations)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("Missing field reservations:");

            foreach (var reservation in reseravations)
            {
                messageBuilder.Append($"Field {reservation.FullyQualifiedName} was removed, but ");

                if (!reservation.NameReserved)
                {
                    messageBuilder.Append($"field name \"{reservation.FieldName}\" was not reserved");
                }

                if (!reservation.NameReserved && !reservation.NumberReserved)
                {
                    messageBuilder.Append(" and ");
                }

                if (!reservation.NumberReserved)
                {
                    messageBuilder.Append($"field number \"{reservation.FieldNumber}\" was not reserved");
                }

                messageBuilder.Append(".");
                messageBuilder.AppendLine();
            }

            return messageBuilder.ToString();
        }

        private class FieldReservation
        {
            public bool NameReserved { get; set; }

            public bool NumberReserved { get; set; }

            public string FullyQualifiedName { get; set; }

            public string FieldName { get; set; }

            public int FieldNumber { get; set; }
        }
    }
}
