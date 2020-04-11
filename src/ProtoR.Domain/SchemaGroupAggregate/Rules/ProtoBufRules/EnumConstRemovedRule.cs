namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using static ProtoR.Domain.SchemaGroupAggregate.Schemas.ProtoBufSchema;

    public class EnumConstRemovedRule : ProtoBufRule
    {
        public EnumConstRemovedRule()
            : base(RuleCode.PB0009)
        {
        }

        public override ValidationResult Validate(ProtoBufSchema a, ProtoBufSchema b)
        {
            a.Should().NotBeNull();
            b.Should().NotBeNull();

            IEnumerable<string> removedEnumConstants = ProtoBufSchemaScope.ParallelTraverse(
                a.RootScope(),
                b.RootScope(),
                this.VisitScope);

            return removedEnumConstants.Any()
                ? new ValidationResult(false, this.FormatRemovedEnumConstants(removedEnumConstants))
                : new ValidationResult(true, "No enum constants were removed");
        }

        private IList<string> VisitScope(ProtoBufSchemaScope a, ProtoBufSchemaScope b)
        {
            var removedEnumConstants = new List<string>();

            foreach (var enumDefinition in b.Enums)
            {
                EnumDescriptorProto matchingEnum = a.Enums.FirstOrDefault(e => e.Name == enumDefinition.Name);

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
                        removedEnumConstants.Add($"{b.Path}.{enumDefinition.Name}.{enumConstant.Number}");
                    }
                }
            }

            return removedEnumConstants;
        }

        private string FormatRemovedEnumConstants(IEnumerable<string> removedEnumConstants)
        {
            return $"Removed enum constants:{Environment.NewLine}{string.Join(Environment.NewLine, removedEnumConstants)}";
        }
    }
}
