namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using static ProtoR.Domain.SchemaGroupAggregate.Schemas.ProtoBufSchema;

    public class EnumRemovedRule : ProtoBufRule
    {
        public EnumRemovedRule()
            : base(RuleCode.PB0003)
        {
        }

        public override ValidationResult Validate(ProtoBufSchema a, ProtoBufSchema b)
        {
            _ = a ?? throw new ArgumentNullException(nameof(a));
            _ = b ?? throw new ArgumentNullException(nameof(b));

            IEnumerable<string> removedEnumTypes = ProtoBufSchemaScope.ParallelTraverse(
                a.RootScope(),
                b.RootScope(),
                this.VisitScope);

            return removedEnumTypes.Any()
                ? new ValidationResult(this.Code, false, this.FormatRemovedEnumTypes(removedEnumTypes))
                : new ValidationResult(this.Code, true, "No Enum types were removed");
        }

        private IList<string> VisitScope(ProtoBufSchemaScope a, ProtoBufSchemaScope b)
        {
            var addedEnums = new List<string>();

            foreach (var enumDefinition in b.Enums)
            {
                EnumDescriptorProto matchingEnum = a.Enums.FirstOrDefault(e => e.Name == enumDefinition.Name);

                if (matchingEnum == null)
                {
                    addedEnums.Add($"{b.Path}.{enumDefinition.Name}");
                }
            }

            return addedEnums;
        }

        private string FormatRemovedEnumTypes(IEnumerable<string> removedEnumTypes)
        {
            return $"Removed Enum types:{Environment.NewLine}{string.Join(Environment.NewLine, removedEnumTypes)}";
        }
    }
}
