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
        public void Validate_WithAddedOneOfField_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("OneOfFieldAdded", "AddedOneOfFieldInNestedMessage")]
        public void Validate_WithAddedOneOfFieldInNestedMessage_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("OneOfFieldAdded", "OneOfFieldMovedFromParentMessage")]
        public void Validate_WithOneOfFieldMovedFromParentMessage_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("OneOfFieldAdded", "OneOfFieldMovedFromAnotherOneOf")]
        public void Validate_WithOneOfFieldMovedFromAnotherOneOf_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("OneOfFieldAdded", "NoAddedOneOfFields")]
        public void Validate_WithNoAddedOneOfFields_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }

        [Theory]
        [RuleTestData("OneOfFieldAdded", "NewOneOf")]
        public void Validate_WithNewOneOf_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }

        [Theory]
        [RuleTestData("OneOfFieldAdded", "NoAddedFieldsAndAnotherOneOfRemoved")]
        public void Validate_WithNoAddedFieldsAndAnotherOneOfRemoved_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }
    }
}
