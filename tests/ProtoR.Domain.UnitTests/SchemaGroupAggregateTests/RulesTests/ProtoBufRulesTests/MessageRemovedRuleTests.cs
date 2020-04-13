namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests.RulesTests.ProtoBufRulesTests
{
    using System;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.UnitTests.SchemaFixtures;
    using Xunit;

    public class MessageRemovedRuleTests
    {
        private readonly MessageRemovedRule rule;

        public MessageRemovedRuleTests()
        {
            this.rule = new MessageRemovedRule();
        }

        [Theory]
        [RuleTestData("MessageRemoved", "RemovedMessage")]
        [RuleTestData("MessageRemoved", "NestedMessage")]
        public void Validate_WithRuleViolation_ShouldNotPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.False(result.Passed);
        }

        [Theory]
        [RuleTestData("MessageRemoved", "NoRemovedMessages")]
        public void Validate_WithNoRemovedMessage_ShouldPass(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.True(result.Passed);
        }

        [Theory]
        [RuleTestData("MessageRemoved", "NestedMessagesRemoved")]
        public void Validate_WithNestedMessagesRemoved_ShouldDetectOnlyOutermostMessage(ProtoBufSchema a, ProtoBufSchema b)
        {
            ValidationResult result = this.rule.Validate(a, b);

            Assert.DoesNotContain("Inner", result.Description, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
