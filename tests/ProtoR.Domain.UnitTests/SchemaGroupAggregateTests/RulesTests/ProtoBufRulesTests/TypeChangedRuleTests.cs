namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests.RulesTests.ProtoBufRulesTests
{
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.UnitTests.SchemaFixtures;
    using Xunit;

    public class TypeChangedRuleTests
    {
        private readonly TypeChangedRule rule;

        public TypeChangedRuleTests()
        {
            this.rule = new TypeChangedRule();
        }

        [Theory]
        [RuleTestData("TypeChanged", "ScalarTypeChanged")]
        [RuleTestData("TypeChanged", "EnumTypeChanged")]
        [RuleTestData("TypeChanged", "MessageTypeChanged")]
        [RuleTestData("TypeChanged", "MapKeyTypeChanged")]
        [RuleTestData("TypeChanged", "MapValueTypeChanged")]
        [RuleTestData("TypeChanged", "MessageTypeChangedToScalar")]
        [RuleTestData("TypeChanged", "ScalarChangedToMessageType")]
        [RuleTestData("TypeChanged", "MessageTypeChangedToEnum")]
        [RuleTestData("TypeChanged", "EnumChangedToMessageType")]
        [RuleTestData("TypeChanged", "ScalarChangedToEnum")]
        [RuleTestData("TypeChanged", "EnumChangedToScalar")]
        [RuleTestData("TypeChanged", "MapChangedToEnum")]
        [RuleTestData("TypeChanged", "EnumChangedToMap")]
        [RuleTestData("TypeChanged", "MapChangedToScalar")]
        [RuleTestData("TypeChanged", "ScalarChangedToMap")]
        [RuleTestData("TypeChanged", "MapChangedToMessageType")]
        [RuleTestData("TypeChanged", "MessageTypeChangedToMap")]
        public void Validate_WithRuleViolation_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("TypeChanged", "NoChangedTypes")]
        public void Validate_WithNoRuleViolation_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }
    }
}
