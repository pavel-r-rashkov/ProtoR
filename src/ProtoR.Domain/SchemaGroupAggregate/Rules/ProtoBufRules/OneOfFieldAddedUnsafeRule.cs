namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using static ProtoR.Domain.SchemaGroupAggregate.Schemas.ProtoBufSchema;

    public class OneOfFieldAddedUnsafeRule : ProtoBufRule
    {
        public OneOfFieldAddedUnsafeRule()
            : base(RuleCode.PB0018)
        {
        }

        public override ValidationResult Validate(ProtoBufSchema a, ProtoBufSchema b)
        {
            a.Should().NotBeNull();
            b.Should().NotBeNull();

            IEnumerable<string> addedOneOfFields = ProtoBufSchemaScope.ParallelTraverse(
                a.RootScope(),
                b.RootScope(),
                this.VisitScope);

            return addedOneOfFields.Any()
                ? new ValidationResult(this.Code, false, this.FormatRemovedOneOfFields(addedOneOfFields))
                : new ValidationResult(this.Code, true, "No OneOf fields were added in unsafe manner");
        }

        private IList<string> VisitScope(ProtoBufSchemaScope a, ProtoBufSchemaScope b)
        {
            var addedOneOfFields = new List<string>();

            foreach (var oneOfField in a.MessageFields.Where(f => (int?)ProtoBufSchemaScope.OneOfIndexField.GetValue(f) != null))
            {
                FieldDescriptorProto matchingOneOfField = b.MessageFields.FirstOrDefault(f => f.Number == oneOfField.Number);

                if (matchingOneOfField == null)
                {
                    continue;
                }

                var destinationOneOfIndex = (int)ProtoBufSchemaScope.OneOfIndexField.GetValue(oneOfField);
                string oneOfName = a.OneOfDefinitions.ElementAt(destinationOneOfIndex).Name;
                OneofDescriptorProto oneOfSource = b.OneOfDefinitions.FirstOrDefault(o => o.Name == oneOfName);

                if (oneOfSource == null)
                {
                    // Containing OneOf is new
                    int movedFieldsCount = a.MessageFields
                        .Where(f => (int?)ProtoBufSchemaScope.OneOfIndexField.GetValue(f) == destinationOneOfIndex
                            && f.Number != oneOfField.Number
                            && b.MessageFields.Any(mf => mf.Number == f.Number))
                        .Count();

                    if (movedFieldsCount == 0)
                    {
                        continue;
                    }
                }
                else if (destinationOneOfIndex == (int?)ProtoBufSchemaScope.OneOfIndexField.GetValue(matchingOneOfField))
                {
                    // Field exist within the same OneOf - the field is unchanged
                    continue;
                }

                addedOneOfFields.Add($"{a.Path}.{oneOfField.Number}");
            }

            return addedOneOfFields;
        }

        private string FormatRemovedOneOfFields(IEnumerable<string> addedOneOfFields)
        {
            return $"Added OneOf fields:{Environment.NewLine}{string.Join(Environment.NewLine, addedOneOfFields)}";
        }
    }
}
