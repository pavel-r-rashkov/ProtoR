namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests.RulesTests.ProtoBufRulesTests
{
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.UnitTests.SchemaFixtures;
    using Xunit;

    public class EnumConstRemovedRuleTests
    {
        private readonly EnumConstRemovedRule rule;

        public EnumConstRemovedRuleTests()
        {
            this.rule = new EnumConstRemovedRule();
        }

        [Theory]
        [RuleTestData("EnumConstRemoved", "RemovedEnumConst")]
        [RuleTestData("EnumConstRemoved", "NestedEnumConstRemoved")]
        public void Validate_WithRuleViolation_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("EnumConstRemoved", "NoRemovedEnumConst")]
        [RuleTestData("EnumConstRemoved", "EnumConstInRemovedEnum")]
        public void Validate_WithNoRuleViolation_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }
    }
}
