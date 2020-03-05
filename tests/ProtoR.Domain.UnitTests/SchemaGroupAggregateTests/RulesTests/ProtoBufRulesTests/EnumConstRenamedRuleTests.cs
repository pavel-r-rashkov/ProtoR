namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests.RulesTests.ProtoBufRulesTests
{
    using System.Diagnostics;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.UnitTests.SchemaFixtures;
    using Xunit;

    public class EnumConstRenamedRuleTests
    {
        private EnumConstRenamedRule rule;

        public EnumConstRenamedRuleTests()
        {
            this.rule = new EnumConstRenamedRule();
        }

        [Theory]
        [RuleTestData("EnumConstRenamed", "RenamedEnumConst")]
        public void Validate_WithRenamedEnumConst_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);
            Debug.WriteLine(result.Description);
            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("EnumConstRenamed", "RenamedEnumConstInNestedMessage")]
        public void Validate_WithRenamedEnumConstInNestedMessage_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);
            Debug.WriteLine(result.Description);
            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("EnumConstRenamed", "NoRenamedEnumConstants")]
        public void Validate_WithNoRenamedEnumConstants_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);
            Debug.WriteLine(result.Description);
            Assert.True(result.Passed);
        }
    }
}
