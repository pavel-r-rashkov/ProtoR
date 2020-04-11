namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests.RulesTests.ProtoBufRulesTests
{
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.UnitTests.SchemaFixtures;
    using Xunit;

    public class OneOfFieldAddedUnsafeRuleTests
    {
        private readonly OneOfFieldAddedUnsafeRule rule;

        public OneOfFieldAddedUnsafeRuleTests()
        {
            this.rule = new OneOfFieldAddedUnsafeRule();
        }

        [Theory]
        [RuleTestData("OneOfFieldAddedUnsafe", "NewFields")]
        public void Validate_WithNewFields_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }

        [Theory]
        [RuleTestData("OneOfFieldAddedUnsafe", "NewOneOfAndSingleMovedField")]
        public void Validate_WithNewOneOfAndSingleMovedField_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }

        [Theory]
        [RuleTestData("OneOfFieldAddedUnsafe", "NewOneOfAndMultipleMovedFields")]
        public void Validate_WithNewOneOfAndMultipleMovedFields_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("OneOfFieldAddedUnsafe", "ExistingOneOfAndSingleMovedField")]
        public void Validate_WithExistingOneOfAndSingleMovedField_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }
    }
}
