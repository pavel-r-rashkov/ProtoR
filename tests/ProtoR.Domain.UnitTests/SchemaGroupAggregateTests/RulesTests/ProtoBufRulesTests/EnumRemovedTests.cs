namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests.RulesTests.ProtoBufRulesTests
{
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.UnitTests.SchemaFixtures;
    using Xunit;

    public class EnumRemovedTests
    {
        private readonly EnumRemovedRule rule;

        public EnumRemovedTests()
        {
            this.rule = new EnumRemovedRule();
        }

        [Theory]
        [RuleTestData("EnumRemoved", "RemovedEnum")]
        [RuleTestData("EnumRemoved", "NestedEnum")]
        public void Validate_WithRuleViolation_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("EnumRemoved", "NoRemovedEnums")]
        [RuleTestData("EnumRemoved", "EnumInRemovedMessage")]
        public void Validate_WithNoRuleViolation_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }
    }
}
