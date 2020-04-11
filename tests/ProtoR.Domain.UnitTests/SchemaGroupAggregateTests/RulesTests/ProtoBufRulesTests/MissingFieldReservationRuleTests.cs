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
        public void Validate_WithNoMissingReservations_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }

        [Theory]
        [RuleTestData("MissingFieldReservation", "MissingReservationInNestedMessage")]
        public void Validate_WithMissingReservationInNestedMessage_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("MissingFieldReservation", "MissingReservation")]
        public void Validate_WithMissingReservation_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("MissingFieldReservation", "MissingReservationInRemovedMessage")]
        public void Validate_WithMissingReservationInRemovedMessage_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }

        [Theory]
        [RuleTestData("MissingFieldReservation", "MissingNumberReservation")]
        public void Validate_WithMissingNumberReservation_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("MissingFieldReservation", "MissingNameReservation")]
        public void Validate_WithMissingNameReservation_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }
    }
}
