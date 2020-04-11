namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using static ProtoR.Domain.SchemaGroupAggregate.Schemas.ProtoBufSchema;

    public class EnumConstRenamedRule : ProtoBufRule
    {
        public EnumConstRenamedRule()
            : base(RuleCode.PB0014)
        {
        }

        public override ValidationResult Validate(ProtoBufSchema a, ProtoBufSchema b)
        {
            a.Should().NotBeNull();
            b.Should().NotBeNull();

            IEnumerable<EnumConstant> renamedEnumConstants = ProtoBufSchemaScope.ParallelTraverse(
                a.RootScope(),
                b.RootScope(),
                this.VisitScope);

            return renamedEnumConstants.Any()
                ? new ValidationResult(false, $"Enum consts were renamed:{Environment.NewLine}{this.FormatRenamedEnumConstants(renamedEnumConstants)}")
                : new ValidationResult(true, "No Enum consts were renamed.");
        }

        private IList<EnumConstant> VisitScope(ProtoBufSchemaScope a, ProtoBufSchemaScope b)
        {
            var renamedEnumConstants = new List<EnumConstant>();

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

                    if (matchingEnumConstant != null && enumConstant.Name != matchingEnumConstant.Name)
                    {
                        renamedEnumConstants.Add(new EnumConstant
                        {
                            FullyQualifiedName = $"{a.Path}.{enumDefinition.Name}.{enumConstant.Number}",
                            Name = enumConstant.Name,
                            RenamedFrom = matchingEnumConstant.Name,
                        });
                    }
                }
            }

            return renamedEnumConstants;
        }

        private string FormatRenamedEnumConstants(IEnumerable<EnumConstant> renamedEnumConstants)
        {
            var messages = renamedEnumConstants.Select(enumConstant => $"{enumConstant.FullyQualifiedName} was renamed from {enumConstant.RenamedFrom} to {enumConstant.Name}");
            return string.Join(Environment.NewLine, messages);
        }

        private class EnumConstant
        {
            public string FullyQualifiedName { get; set; }

            public string Name { get; set; }

            public string RenamedFrom { get; set; }
        }
    }
}
