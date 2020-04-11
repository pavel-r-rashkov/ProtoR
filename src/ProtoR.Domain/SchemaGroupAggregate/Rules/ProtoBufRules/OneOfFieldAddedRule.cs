namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using static ProtoR.Domain.SchemaGroupAggregate.Schemas.ProtoBufSchema;

    public class OneOfFieldAddedRule : ProtoBufRule
    {
        public OneOfFieldAddedRule()
            : base(RuleCode.PB0017)
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
                ? new ValidationResult(false, this.FormatRemovedOneOfFields(addedOneOfFields))
                : new ValidationResult(true, "No OneOf fields were added");
        }

        private IList<string> VisitScope(ProtoBufSchemaScope a, ProtoBufSchemaScope b)
        {
            var addedOneOfFields = new List<string>();

            foreach (var oneOfField in a.MessageFields.Where(f => (int?)ProtoBufSchemaScope.OneOfIndexField.GetValue(f) != null))
            {
                FieldDescriptorProto matchingOneOfField = b.MessageFields.FirstOrDefault(f =>
                    f.Number == oneOfField.Number
                    && (int?)ProtoBufSchemaScope.OneOfIndexField.GetValue(oneOfField) == (int?)ProtoBufSchemaScope.OneOfIndexField.GetValue(f));

                if (matchingOneOfField == null)
                {
                    addedOneOfFields.Add($"{a.Path}.{oneOfField.Number}");
                }
            }

            return addedOneOfFields;
        }

        private string FormatRemovedOneOfFields(IEnumerable<string> addedOneOfFields)
        {
            return $"Added OneOf fields:{Environment.NewLine}{string.Join(Environment.NewLine, addedOneOfFields)}";
        }
    }
}
