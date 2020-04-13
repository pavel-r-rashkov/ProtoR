namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests.RulesTests.ProtoBufRulesTests
{
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.UnitTests.SchemaFixtures;
    using Xunit;

    public class EnumAddedRuleTests
    {
        private readonly EnumAddedRule rule;

        public EnumAddedRuleTests()
        {
            this.rule = new EnumAddedRule();
        }

        [Theory]
        [RuleTestData("EnumAdded", "AddedEnum")]
        [RuleTestData("EnumAdded", "InnerEnum")]
        public void Validate_WithARuleViolation_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("EnumAdded", "NoAddedEnum")]
        [RuleTestData("EnumAdded", "EnumInNewMessage")]
        public void Validate_WithNoRuleViolation_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }
    }
}
