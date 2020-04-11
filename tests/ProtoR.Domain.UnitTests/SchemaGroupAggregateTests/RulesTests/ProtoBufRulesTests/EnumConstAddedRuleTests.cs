namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests.RulesTests.ProtoBufRulesTests
{
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.UnitTests.SchemaFixtures;
    using Xunit;

    public class EnumConstAddedRuleTests
    {
        private readonly EnumConstAddedRule rule;

        public EnumConstAddedRuleTests()
        {
            this.rule = new EnumConstAddedRule();
        }

        [Theory]
        [RuleTestData("EnumConstAdded", "AddedEnumConst")]
        public void Validate_WithAddedEnumConst_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("EnumConstAdded", "NestedEnumConstAdded")]
        public void Validate_WithAddedNestedEnumConst_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("EnumConstAdded", "NoAddedEnumConst")]
        public void Validate_WithNoAddedEnumConst_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }

        [Theory]
        [RuleTestData("EnumConstAdded", "EnumConstInNewEnum")]
        public void Validate_WithEnumConstInNewEnum_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }
    }
}
