namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests.RulesTests.ProtoBufRulesTests
{
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.UnitTests.SchemaFixtures;
    using Xunit;

    public class FieldRemovedRuleTests
    {
        private readonly FieldRemovedRule rule;

        public FieldRemovedRuleTests()
        {
            this.rule = new FieldRemovedRule();
        }

        [Theory]
        [RuleTestData("FieldRemoved", "RemovedField")]
        [RuleTestData("FieldRemoved", "NestedMessageField")]
        public void Validate_WithRuleViolation_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("FieldRemoved", "NoRemovedFields")]
        [RuleTestData("FieldRemoved", "FieldInRemovedMessage")]
        public void Validate_WithNoRuleViolation_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }
    }
}
