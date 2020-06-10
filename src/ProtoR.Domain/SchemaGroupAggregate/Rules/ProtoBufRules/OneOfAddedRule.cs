namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using static ProtoR.Domain.SchemaGroupAggregate.Schemas.ProtoBufSchema;

    public class OneOfAddedRule : ProtoBufRule
    {
        public OneOfAddedRule()
            : base(RuleCode.PB0006)
        {
        }

        public override ValidationResult Validate(ProtoBufSchema a, ProtoBufSchema b)
        {
            _ = a ?? throw new ArgumentNullException(nameof(a));
            _ = b ?? throw new ArgumentNullException(nameof(b));

            IEnumerable<string> removedOneOfTypes = ProtoBufSchemaScope.ParallelTraverse(
                a.RootScope(),
                b.RootScope(),
                this.VisitScope);

            return removedOneOfTypes.Any()
                ? new ValidationResult(this.Code, false, this.FormatRemovedOneOfTypes(removedOneOfTypes))
                : new ValidationResult(this.Code, true, "No OneOf types were added");
        }

        private IList<string> VisitScope(ProtoBufSchemaScope a, ProtoBufSchemaScope b)
        {
            var addedOneOfDefinitions = new List<string>();

            foreach (var oneOfDefinition in a.OneOfDefinitions)
            {
                OneofDescriptorProto matchingOneOf = b.OneOfDefinitions.FirstOrDefault(e => e.Name == oneOfDefinition.Name);

                if (matchingOneOf == null)
                {
                    addedOneOfDefinitions.Add($"{a.Path}.{oneOfDefinition.Name}");
                }
            }

            return addedOneOfDefinitions;
        }

        private string FormatRemovedOneOfTypes(IEnumerable<string> removedOneOfTypes)
        {
            return $"Added OneOf types:{Environment.NewLine}{string.Join(Environment.NewLine, removedOneOfTypes)}";
        }
    }
}
