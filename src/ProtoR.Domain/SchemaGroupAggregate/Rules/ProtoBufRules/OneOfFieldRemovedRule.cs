namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using static ProtoR.Domain.SchemaGroupAggregate.Schemas.ProtoBufSchema;

    public class OneOfFieldRemovedRule : ProtoBufRule
    {
        public OneOfFieldRemovedRule()
            : base(RuleCode.PB0016)
        {
        }

        public override ValidationResult Validate(ProtoBufSchema a, ProtoBufSchema b)
        {
            a.Should().NotBeNull();
            b.Should().NotBeNull();

            IEnumerable<string> removedOneOfFields = ProtoBufSchemaScope.ParallelTraverse(
                a.RootScope(),
                b.RootScope(),
                this.VisitScope);

            return removedOneOfFields.Any()
                ? new ValidationResult(false, this.FormatRemovedOneOfFields(removedOneOfFields))
                : new ValidationResult(true, "No OneOf fields were removed");
        }

        private IList<string> VisitScope(ProtoBufSchemaScope a, ProtoBufSchemaScope b)
        {
            var removedOneOfFields = new List<string>();

            foreach (var oneOfField in b.MessageFields.Where(f => (int?)ProtoBufSchemaScope.OneOfIndexField.GetValue(f) != null))
            {
                string sourceOneOfName = b.OneOfDefinitions.ElementAt((int)ProtoBufSchemaScope.OneOfIndexField.GetValue(oneOfField)).Name;
                OneofDescriptorProto destinationOneOf = a.OneOfDefinitions.FirstOrDefault(o => o.Name == sourceOneOfName);

                if (destinationOneOf == null)
                {
                    continue;
                }

                int destinationOneOfIndex = a.OneOfDefinitions.ToList().IndexOf(destinationOneOf);

                FieldDescriptorProto matchingOneOfField = a.MessageFields.FirstOrDefault(f =>
                    f.Number == oneOfField.Number
                    && (int?)ProtoBufSchemaScope.OneOfIndexField.GetValue(f) == destinationOneOfIndex);

                if (matchingOneOfField == null)
                {
                    removedOneOfFields.Add($"{b.Path}.{oneOfField.Number}");
                }
            }

            return removedOneOfFields;
        }

        private string FormatRemovedOneOfFields(IEnumerable<string> removedOneOfFields)
        {
            return $"Removed OneOf fields:{Environment.NewLine}{string.Join(Environment.NewLine, removedOneOfFields)}";
        }
    }
}
