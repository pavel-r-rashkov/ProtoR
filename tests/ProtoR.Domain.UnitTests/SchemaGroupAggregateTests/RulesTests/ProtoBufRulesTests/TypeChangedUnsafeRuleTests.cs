namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests.RulesTests.ProtoBufRulesTests
{
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.UnitTests.SchemaFixtures;
    using Xunit;

    public class TypeChangedUnsafeRuleTests
    {
        private readonly TypeChangedUnsafeRule rule;

        public TypeChangedUnsafeRuleTests()
        {
            this.rule = new TypeChangedUnsafeRule();
        }

        [Theory]
        [RuleTestData("TypeChangedUnsafe", "IncompatibleScalarTypeChange")]
        [RuleTestData("TypeChangedUnsafe", "EnumToIncompatibleScalarTypeChange")]
        [RuleTestData("TypeChangedUnsafe", "MapKeyTypeIncompatibleChange")]
        [RuleTestData("TypeChangedUnsafe", "MapValueTypeIncompatibleChange")]
        [RuleTestData("TypeChangedUnsafe", "MessageTypeChanged")]
        public void Validate_WithRuleViolation_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("TypeChangedUnsafe", "ScalarNumberTypeChanged")]
        [RuleTestData("TypeChangedUnsafe", "ScalarSignedNumberTypeChanged")]
        [RuleTestData("TypeChangedUnsafe", "ScalarStringOrBytesTypeChanged")]
        [RuleTestData("TypeChangedUnsafe", "ScalarFixed32TypeChanged")]
        [RuleTestData("TypeChangedUnsafe", "ScalarFixed64TypeChanged")]
        [RuleTestData("TypeChangedUnsafe", "EnumToCompatibleScalarTypeChange")]
        [RuleTestData("TypeChangedUnsafe", "MapKeyTypeCompatibleChange")]
        [RuleTestData("TypeChangedUnsafe", "MapValueTypeCompatibleChange")]
        public void Validate_WithNoRuleViolation_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }
    }
}
