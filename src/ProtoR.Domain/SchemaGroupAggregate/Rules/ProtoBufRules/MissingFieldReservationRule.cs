namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using FluentAssertions;
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;

    public class MissingFieldReservationRule : ProtoBufRule
    {
        public MissingFieldReservationRule()
            : base(RuleCode.PB0015)
        {
        }

        public override ValidationResult Validate(ProtoBufSchema a, ProtoBufSchema b)
        {
            a.Should().NotBeNull();
            b.Should().NotBeNull();

            IEnumerable<string> aFields = a.GetFieldNumbers();
            IEnumerable<string> bFields = b.GetFieldNumbers();
            IEnumerable<string> removedFieldNames = bFields.Except(aFields);
            var reservations = new List<FieldReservation>();

            foreach (var removedFieldName in removedFieldNames)
            {
                DescriptorProto originalMessage = this.FindMessage(b, removedFieldName);
                int fieldNumber = Convert.ToInt32(removedFieldName.Substring(removedFieldName.LastIndexOf('.') + 1), CultureInfo.InvariantCulture);
                FieldDescriptorProto removedField = originalMessage.Fields.First(f => f.Number == fieldNumber);
                DescriptorProto message = this.FindMessage(a, removedFieldName);

                if (message == null)
                {
                    continue;
                }

                reservations.Add(this.CheckReservationForField(
                    removedFieldName,
                    removedField.Name,
                    fieldNumber,
                    message));
            }

            IEnumerable<FieldReservation> missingReservations = reservations
                .Where(r => !r.NameReserved || !r.NumberReserved);

            return missingReservations.Any()
                ? new ValidationResult(false, this.FormatMissingReservations(missingReservations))
                : new ValidationResult(true, "No field reservations are missing.");
        }

        private FieldReservation CheckReservationForField(
            string fullyQualifiedName,
            string removedFieldName,
            int removedFieldNumber,
            DescriptorProto message)
        {
            bool nameReserved = message.ReservedNames.Contains(removedFieldName);
            bool numberReserved = message.ReservedRanges.Any(range => range.Start <= removedFieldNumber && removedFieldNumber < range.End);

            return new FieldReservation
            {
                NameReserved = nameReserved,
                NumberReserved = numberReserved,
                FullyQualifiedName = fullyQualifiedName,
                FieldName = removedFieldName,
                FieldNumber = removedFieldNumber,
            };
        }

        private DescriptorProto FindMessage(ProtoBufSchema schema, string fullyQualifiedName)
        {
            string[] names = fullyQualifiedName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            int fieldNumber = Convert.ToInt32(names.Last(), CultureInfo.InvariantCulture);
            IEnumerable<string> messageNames = names.Take(names.Length - 1);
            List<DescriptorProto> messageDescriptors = schema.Parsed.Files.First().MessageTypes;
            DescriptorProto parentMessage = null;

            while (names.Length > 1)
            {
                string messageName = names[0];
                names = names.Skip(1).ToArray();
                parentMessage = messageDescriptors.FirstOrDefault(m => m.Name == messageName);

                if (parentMessage == null)
                {
                    return null;
                }

                messageDescriptors = parentMessage.NestedTypes;
            }

            return parentMessage;
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
