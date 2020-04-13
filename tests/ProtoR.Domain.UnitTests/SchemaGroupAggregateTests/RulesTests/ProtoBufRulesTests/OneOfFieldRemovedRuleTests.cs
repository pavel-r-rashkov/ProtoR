namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests.RulesTests.ProtoBufRulesTests
{
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.UnitTests.SchemaFixtures;
    using Xunit;

    public class OneOfFieldRemovedRuleTests
    {
        private readonly OneOfFieldRemovedRule rule;

        public OneOfFieldRemovedRuleTests()
        {
            this.rule = new OneOfFieldRemovedRule();
        }

        [Theory]
        [RuleTestData("OneOfFieldRemoved", "RemovedOneOfField")]
        [RuleTestData("OneOfFieldRemoved", "RemovedOneOfFieldInNestedMessage")]
        [RuleTestData("OneOfFieldRemoved", "OneOfFieldMovedToParentMessage")]
        [RuleTestData("OneOfFieldRemoved", "OneOfFieldMovedToAnotherOneOf")]
        public void Validate_WithRuleViolation_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("OneOfFieldRemoved", "NoRemovedOneOfFields")]
        [RuleTestData("OneOfFieldRemoved", "RemovedParentOneOf")]
        [RuleTestData("OneOfFieldRemoved", "NoRemovedFieldsAndAnotherOneOfRemoved")]
        public void Validate_WithNoRuleViolation_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }
    }
}
