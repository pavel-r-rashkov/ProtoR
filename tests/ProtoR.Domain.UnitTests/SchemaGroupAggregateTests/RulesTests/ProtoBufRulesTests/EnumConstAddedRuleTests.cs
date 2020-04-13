namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests.RulesTests.ProtoBufRulesTests
{
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.UnitTests.SchemaFixtures;
    using Xunit;

    public class EnumConstAddedRuleTests
    {
        private readonly EnumConstAddedRule rule;

        public EnumConstAddedRuleTests()
        {
            this.rule = new EnumConstAddedRule();
        }

        [Theory]
        [RuleTestData("EnumConstAdded", "AddedEnumConst")]
        [RuleTestData("EnumConstAdded", "NestedEnumConstAdded")]
        public void Validate_WithRuleViolation_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("EnumConstAdded", "NoAddedEnumConst")]
        [RuleTestData("EnumConstAdded", "EnumConstInNewEnum")]
        public void Validate_WithNoRuleViolation_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }
    }
}
