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
        public void Validate_WithRemovedOneOfField_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("OneOfFieldRemoved", "RemovedOneOfFieldInNestedMessage")]
        public void Validate_WithRemovedOneOfFieldInNestedMessage_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("OneOfFieldRemoved", "OneOfFieldMovedToParentMessage")]
        public void Validate_WithOneOfFieldMovedToParentMessage_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("OneOfFieldRemoved", "OneOfFieldMovedToAnotherOneOf")]
        public void Validate_WithOneOfFieldMovedToAnotherOneOf_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("OneOfFieldRemoved", "NoRemovedOneOfFields")]
        public void Validate_WithNoRemovedOneOfFields_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }
    }
}
