namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests.RulesTests
{
    using ProtoR.Domain.ConfigurationAggregate;
    using Xunit;

    public class RuleConfigTests
    {
        [Fact]
        public void RuleConfig_ShoudBeCreated()
        {
            var ruleConfig = new RuleConfiguration(false, Severity.Error);

            Assert.NotNull(ruleConfig);
        }

        [Fact]
        public void Equality_WithEqualProperties_ShouldBeTrue()
        {
            var firstConfig = new RuleConfiguration(false, Severity.Error);
            var secondConfig = new RuleConfiguration(false, Severity.Error);

            Assert.Equal(firstConfig, secondConfig);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void WithInheritance_ShouldReturnValueWithInheritanceSet(bool inherit)
        {
            var originalConfig = new RuleConfiguration(inherit, Severity.Error);

            RuleConfiguration config = originalConfig.WithInheritance(!inherit);

            Assert.Equal(!inherit, config.Inherit);
        }
    }
}
