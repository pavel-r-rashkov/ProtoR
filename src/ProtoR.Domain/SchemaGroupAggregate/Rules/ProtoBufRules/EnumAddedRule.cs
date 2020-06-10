namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using static ProtoR.Domain.SchemaGroupAggregate.Schemas.ProtoBufSchema;

    public class EnumAddedRule : ProtoBufRule
    {
        public EnumAddedRule()
            : base(RuleCode.PB0004)
        {
        }

        public override ValidationResult Validate(ProtoBufSchema a, ProtoBufSchema b)
        {
            _ = a ?? throw new ArgumentNullException(nameof(a));
            _ = b ?? throw new ArgumentNullException(nameof(b));

            IEnumerable<string> addedEnumTypes = ProtoBufSchemaScope.ParallelTraverse(a.RootScope(), b.RootScope(), this.VisitScope);

            return addedEnumTypes.Any()
                ? new ValidationResult(this.Code, false, this.FormatRemovedEnumTypes(addedEnumTypes))
                : new ValidationResult(this.Code, true, "No enum types were added");
        }

        private IList<string> VisitScope(ProtoBufSchemaScope a, ProtoBufSchemaScope b)
        {
            var addedEnums = new List<string>();

            foreach (var enumDefinition in a.Enums)
            {
                EnumDescriptorProto matchingEnum = b.Enums.FirstOrDefault(e => e.Name == enumDefinition.Name);

                if (matchingEnum == null)
                {
                    addedEnums.Add($"{b.Path}.{enumDefinition.Name}");
                }
            }

            return addedEnums;
        }

        private string FormatRemovedEnumTypes(IEnumerable<string> addedEnumTypes)
        {
            return $"Added enum types:{Environment.NewLine}{string.Join(Environment.NewLine, addedEnumTypes)}";
        }
    }
}
