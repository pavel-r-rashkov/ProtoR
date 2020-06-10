namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using static ProtoR.Domain.SchemaGroupAggregate.Schemas.ProtoBufSchema;

    public class EnumConstAddedRule : ProtoBufRule
    {
        public EnumConstAddedRule()
            : base(RuleCode.PB0010)
        {
        }

        public override ValidationResult Validate(ProtoBufSchema a, ProtoBufSchema b)
        {
            _ = a ?? throw new ArgumentNullException(nameof(a));
            _ = b ?? throw new ArgumentNullException(nameof(b));

            IEnumerable<string> addedEnumConstants = ProtoBufSchemaScope.ParallelTraverse(
                a.RootScope(),
                b.RootScope(),
                this.VisitScope);

            return addedEnumConstants.Any()
                ? new ValidationResult(this.Code, false, this.FormatAddedEnumConstants(addedEnumConstants))
                : new ValidationResult(this.Code, true, "No enum constants were added");
        }

        private IList<string> VisitScope(ProtoBufSchemaScope a, ProtoBufSchemaScope b)
        {
            var addedEnumConstants = new List<string>();

            foreach (var enumDefinition in a.Enums)
            {
                EnumDescriptorProto matchingEnum = b.Enums.FirstOrDefault(e => e.Name == enumDefinition.Name);

                if (matchingEnum == null)
                {
                    continue;
                }

                foreach (var enumConstant in enumDefinition.Values)
                {
                    EnumValueDescriptorProto matchingEnumConstant = matchingEnum.Values
                        .FirstOrDefault(e => e.Number == enumConstant.Number);

                    if (matchingEnumConstant == null)
                    {
                        addedEnumConstants.Add($"{a.Path}.{enumDefinition.Name}.{enumConstant.Number}");
                    }
                }
            }

            return addedEnumConstants;
        }

        private string FormatAddedEnumConstants(IEnumerable<string> addedEnumConstants)
        {
            return $"Added enum constants:{Environment.NewLine}{string.Join(Environment.NewLine, addedEnumConstants)}";
        }
    }
}
