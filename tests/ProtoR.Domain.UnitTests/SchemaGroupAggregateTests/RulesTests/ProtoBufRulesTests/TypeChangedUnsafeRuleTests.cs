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
        public void Validate_WithIncompatibleScalarTypeChange_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("TypeChangedUnsafe", "ScalarNumberTypeChanged")]
        public void Validate_WithScalarNumberTypeChanged_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }

        [Theory]
        [RuleTestData("TypeChangedUnsafe", "ScalarSignedNumberTypeChanged")]
        public void Validate_WithScalarSignedNumberTypeChanged_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }

        [Theory]
        [RuleTestData("TypeChangedUnsafe", "ScalarStringOrBytesTypeChanged")]
        public void Validate_WithScalarStringOrBytesTypeChanged_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }

        [Theory]
        [RuleTestData("TypeChangedUnsafe", "ScalarFixed32TypeChanged")]
        public void Validate_WithScalarFixed32TypeChanged_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }

        [Theory]
        [RuleTestData("TypeChangedUnsafe", "ScalarFixed64TypeChanged")]
        public void Validate_WithScalarFixed64TypeChanged_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }

        [Theory]
        [RuleTestData("TypeChangedUnsafe", "EnumToCompatibleScalarTypeChange")]
        public void Validate_WithEnumToCompatibleScalarTypeChange_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }

        [Theory]
        [RuleTestData("TypeChangedUnsafe", "EnumToIncompatibleScalarTypeChange")]
        public void Validate_WithEnumToIncompatibleScalarTypeChange_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("TypeChangedUnsafe", "MapKeyTypeCompatibleChange")]
        public void Validate_WithMapKeyTypeCompatibleChange_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }

        [Theory]
        [RuleTestData("TypeChangedUnsafe", "MapKeyTypeIncompatibleChange")]
        public void Validate_WithMapKeyTypeIncompatibleChange_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("TypeChangedUnsafe", "MapValueTypeCompatibleChange")]
        public void Validate_WithMapValueTypeCompatibleChange_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }

        [Theory]
        [RuleTestData("TypeChangedUnsafe", "MapValueTypeIncompatibleChange")]
        public void Validate_WithMapValueTypeIncompatibleChange_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("TypeChangedUnsafe", "MessageTypeChanged")]
        public void Validate_WithMessageTypeChanged_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }
    }
}
