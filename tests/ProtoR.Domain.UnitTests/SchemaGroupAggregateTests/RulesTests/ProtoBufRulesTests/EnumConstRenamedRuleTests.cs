namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests.RulesTests.ProtoBufRulesTests
{
    using System.Diagnostics;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.UnitTests.SchemaFixtures;
    using Xunit;

    public class EnumConstRenamedRuleTests
    {
        private readonly EnumConstRenamedRule rule;

        public EnumConstRenamedRuleTests()
        {
            this.rule = new EnumConstRenamedRule();
        }

        [Theory]
        [RuleTestData("EnumConstRenamed", "RenamedEnumConst")]
        [RuleTestData("EnumConstRenamed", "RenamedEnumConstInNestedMessage")]
        public void Validate_WithRuleViolation_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("EnumConstRenamed", "NoRenamedEnumConstants")]
        public void Validate_WithNoRuleViolation_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }
    }
}
