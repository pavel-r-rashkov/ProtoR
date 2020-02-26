namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests.RulesTests
{
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using Xunit;

    public class RuleViolationTests
    {
        [Fact]
        public void RuleViolation_ShouldBeCreated()
        {
            var ruleViolation = new RuleViolation(
                new ValidationResult(true, "Description"),
                Severity.Error,
                new Version(1),
                new Version(2),
                true);

            Assert.NotNull(ruleViolation);
        }

        [Fact]
        public void Equality_WithEqualProperties_ShouldBeTrue()
        {
            var firstRuleViolation = new RuleViolation(
                new ValidationResult(true, "Description"),
                Severity.Error,
                new Version(1),
                new Version(2),
                true);

            var secondRuleViolation = new RuleViolation(
                new ValidationResult(true, "Description"),
                Severity.Error,
                new Version(1),
                new Version(2),
                true);

            Assert.Equal(firstRuleViolation, secondRuleViolation);
        }
    }
}
