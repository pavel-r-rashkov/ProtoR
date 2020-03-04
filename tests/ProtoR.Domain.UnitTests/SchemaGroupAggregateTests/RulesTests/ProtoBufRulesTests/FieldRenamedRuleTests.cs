namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests.RulesTests.ProtoBufRulesTests
{
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.UnitTests.SchemaFixtures;
    using Xunit;

    public class FieldRenamedRuleTests
    {
        private FieldRenamedRule rule;

        public FieldRenamedRuleTests()
        {
            this.rule = new FieldRenamedRule();
        }

        [Theory]
        [RuleTestData("FieldRenamed", "RenamedField")]
        public void Validate_WithRenamedField_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("FieldRenamed", "RenamedFieldInNestedMessage")]
        public void Validate_WithRenamedFieldInNestedMessage_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("FieldRenamed", "NoRenamedFields")]
        public void Validate_WithNoRenamedFields_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }
    }
}
