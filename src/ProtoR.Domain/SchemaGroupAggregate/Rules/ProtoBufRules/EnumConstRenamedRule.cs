namespace ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Google.Protobuf.Reflection;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;

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

            IEnumerable<EnumConstant> aEnumConstants = this.GetEnumConstants(a);
            IEnumerable<EnumConstant> bEnumConstants = this.GetEnumConstants(b);

            var renamedEnumConstants = new List<EnumConstant>();

            foreach (var enumConstant in aEnumConstants)
            {
                EnumConstant matchingEnumConstant = bEnumConstants.FirstOrDefault(e => e.FullyQualifiedName == enumConstant.FullyQualifiedName);

                if (matchingEnumConstant != null && matchingEnumConstant.Name != enumConstant.Name)
                {
                    enumConstant.RenamedFrom = matchingEnumConstant.Name;
                    renamedEnumConstants.Add(enumConstant);
                }
            }

            return renamedEnumConstants.Any()
                ? new ValidationResult(false, $"Enum consts were renamed:{Environment.NewLine}{this.FormatRenamedEnumConstants(renamedEnumConstants)}")
                : new ValidationResult(true, "No Enum consts were renamed.");
        }

        private string FormatRenamedEnumConstants(List<EnumConstant> renamedEnumConstants)
        {
            var messages = renamedEnumConstants.Select(enumConstant => $"{enumConstant.FullyQualifiedName} was renamed from {enumConstant.RenamedFrom} to {enumConstant.Name}");
            return string.Join(Environment.NewLine, messages);
        }

        private IEnumerable<EnumConstant> GetEnumConstants(ProtoBufSchema schema)
        {
            IEnumerable<EnumConstant> outerEnumConstants = schema.Parsed.Files
                .SelectMany(f => f.EnumTypes
                    .SelectMany(e => e.Values
                        .Select(ev => new EnumConstant
                        {
                            FullyQualifiedName = $".{e.Name}.{ev.Number}",
                            Name = ev.Name,
                        })));

            IEnumerable<EnumConstant> innerEnumConstants = schema.Parsed.Files
                .SelectMany(f => f.MessageTypes
                    .SelectMany(message => this.GetEnumConstants(message)));

            return outerEnumConstants.Union(innerEnumConstants);
        }

        private IEnumerable<EnumConstant> GetEnumConstants(DescriptorProto message, string parentName = "")
        {
            var messageName = $"{parentName}.{message.Name}";

            IEnumerable<EnumConstant> enumConstants = message.EnumTypes
                .SelectMany(e => e.Values
                    .Select(ev => new EnumConstant
                    {
                        FullyQualifiedName = $"{messageName}.{e.Name}.{ev.Number}",
                        Name = ev.Name,
                    }));

            IEnumerable<EnumConstant> nestedEnumConstants = message.NestedTypes
                .SelectMany(nestedMessage => this.GetEnumConstants(nestedMessage, messageName));

            return enumConstants.Union(nestedEnumConstants);
        }

        private class EnumConstant
        {
            public string FullyQualifiedName { get; set; }

            public string Name { get; set; }

            public string RenamedFrom { get; set; }
        }
    }
}
