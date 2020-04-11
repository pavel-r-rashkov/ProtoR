namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests.RulesTests.ProtoBufRulesTests
{
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.UnitTests.SchemaFixtures;
    using Xunit;

    public class EnumConstRemovedRuleTests
    {
        private readonly EnumConstRemovedRule rule;

        public EnumConstRemovedRuleTests()
        {
            this.rule = new EnumConstRemovedRule();
        }

        [Theory]
        [RuleTestData("EnumConstRemoved", "RemovedEnumConst")]
        public void Validate_WithRemovedEnumConst_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("EnumConstRemoved", "NestedEnumConstRemoved")]
        public void Validate_WithRemovedNestedEnumConst_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("EnumConstRemoved", "NoRemovedEnumConst")]
        public void Validate_WithNoRemovedEnumConst_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }

        [Theory]
        [RuleTestData("EnumConstRemoved", "EnumConstInRemovedEnum")]
        public void Validate_WithEnumConstInRemovedMessage_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }
    }
}
