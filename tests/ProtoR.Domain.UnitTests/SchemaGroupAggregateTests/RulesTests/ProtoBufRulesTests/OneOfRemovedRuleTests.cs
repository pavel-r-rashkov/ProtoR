namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests.RulesTests.ProtoBufRulesTests
{
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.UnitTests.SchemaFixtures;
    using Xunit;

    public class OneOfRemovedRuleTests
    {
        private readonly OneOfRemovedRule rule;

        public OneOfRemovedRuleTests()
        {
            this.rule = new OneOfRemovedRule();
        }

        [Theory]
        [RuleTestData("OneOfRemoved", "RemovedOneOf")]
        public void Validate_WithRemovedOneOf_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("OneOfRemoved", "InnerOneOf")]
        public void Validate_WithRemovedNestedOneOf_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("OneOfRemoved", "NoRemovedOneOf")]
        public void Validate_WithNoRemovedOneOf_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }

        [Theory]
        [RuleTestData("OneOfRemoved", "OneOfInRemovedMessage")]
        public void Validate_WithOneOfInRemovedMessage_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }
    }
}
