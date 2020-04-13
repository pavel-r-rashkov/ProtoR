namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests.RulesTests.ProtoBufRulesTests
{
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.UnitTests.SchemaFixtures;
    using Xunit;

    public class MissingFieldReservationRuleTests
    {
        private readonly MissingFieldReservationRule rule;

        public MissingFieldReservationRuleTests()
        {
            this.rule = new MissingFieldReservationRule();
        }

        [Theory]
        [RuleTestData("MissingFieldReservation", "NoMissingReservations")]
        [RuleTestData("MissingFieldReservation", "MissingReservationInRemovedMessage")]
        public void Validate_WithNoRuleViolation_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }

        [Theory]
        [RuleTestData("MissingFieldReservation", "MissingReservationInNestedMessage")]
        [RuleTestData("MissingFieldReservation", "MissingReservation")]
        [RuleTestData("MissingFieldReservation", "MissingNumberReservation")]
        [RuleTestData("MissingFieldReservation", "MissingNameReservation")]
        public void Validate_WithRuleViolation_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }
    }
}
