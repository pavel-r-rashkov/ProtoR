namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests.RulesTests.ProtoBufRulesTests
{
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.UnitTests.SchemaFixtures;
    using Xunit;

    public class OneOfFieldAddedRuleTests
    {
        private readonly OneOfFieldAddedRule rule;

        public OneOfFieldAddedRuleTests()
        {
            this.rule = new OneOfFieldAddedRule();
        }

        [Theory]
        [RuleTestData("OneOfFieldAdded", "AddedOneOfField")]
        [RuleTestData("OneOfFieldAdded", "AddedOneOfFieldInNestedMessage")]
        [RuleTestData("OneOfFieldAdded", "OneOfFieldMovedFromParentMessage")]
        [RuleTestData("OneOfFieldAdded", "OneOfFieldMovedFromAnotherOneOf")]
        public void Validate_WithRuleViolation_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("OneOfFieldAdded", "NoAddedOneOfFields")]
        [RuleTestData("OneOfFieldAdded", "NewOneOf")]
        [RuleTestData("OneOfFieldAdded", "NoAddedFieldsAndAnotherOneOfRemoved")]
        public void Validate_WithNoRuleViolation_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }
    }
}
