namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests.RulesTests
{
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using Xunit;

    public class RuleConfigTests
    {
        [Fact]
        public void RuleConfig_ShoudBeCreated()
        {
            var ruleConfig = new RuleConfig(false, Severity.Error);

            Assert.NotNull(ruleConfig);
        }

        [Fact]
        public void Equality_WithEqualProperties_ShouldBeTrue()
        {
            var firstConfig = new RuleConfig(false, Severity.Error);
            var secondConfig = new RuleConfig(false, Severity.Error);

            Assert.Equal(firstConfig, secondConfig);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void WithInheritance_ShouldReturnValueWithInheritanceSet(bool shouldInherit)
        {
            var originalConfig = new RuleConfig(shouldInherit, Severity.Error);

            RuleConfig config = originalConfig.WithInheritance(!shouldInherit);

            Assert.Equal(!shouldInherit, config.ShouldInherit);
        }
    }
}
