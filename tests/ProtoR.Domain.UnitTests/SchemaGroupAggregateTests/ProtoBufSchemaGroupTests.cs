namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.Exceptions;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.UnitTests.SchemaFixtures;
    using Xunit;
    using Version = ProtoR.Domain.SchemaGroupAggregate.Schemas.Version;

    public class ProtoBufSchemaGroupTests
    {
        private readonly Fixture fixture;
        private readonly string validSchema;
        private readonly string baseSchema;
        private readonly string schemaWithNewMessage;
        private readonly string schemaWithRemovedMessage;

        public ProtoBufSchemaGroupTests()
        {
            this.fixture = new Fixture();
            this.validSchema = SchemaFixtureUtils.GetProtoBuf("ValidSchema");
            this.baseSchema = SchemaFixtureUtils.GetProtoBuf("CompatibilityBaseSchema");
            this.schemaWithNewMessage = SchemaFixtureUtils.GetProtoBuf("CompatibilityWithNewMessageSchema");
            this.schemaWithRemovedMessage = SchemaFixtureUtils.GetProtoBuf("CompatibilityWithRemovedMessageSchema");
        }

        [Fact]
        public void Name_ShouldReturnSchemaGroupName()
        {
            var name = this.fixture.Create<string>();
            var schemaGroup = new ProtoBufSchemaGroup(name);

            Assert.Equal(name, schemaGroup.Name);
        }

        [Fact]
        public void AddSchema_WithNullGroupConfiguration_ShouldThrow()
        {
            var schemaGroup = this.fixture.Create<ProtoBufSchemaGroup>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                schemaGroup.AddSchema(string.Empty, null, new Dictionary<RuleCode, RuleConfiguration>());
            });
        }

        [Fact]
        public void AddSchema_WithNullRuleConfiguration_ShouldThrow()
        {
            var schemaGroup = this.fixture.Create<ProtoBufSchemaGroup>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                schemaGroup.AddSchema(string.Empty, new GroupConfiguration(true, false, false, false), null);
            });
        }

        [Fact]
        public void AddSchema_WithInvalidContents_ShouldThrow()
        {
            var schemaGroup = this.fixture.Create<ProtoBufSchemaGroup>();

            Assert.Throws<InvalidProtoBufSchemaException>(() =>
            {
                schemaGroup.AddSchema(
                    string.Empty,
                    new GroupConfiguration(true, false, false, false),
                    new Dictionary<RuleCode, RuleConfiguration>());
            });
        }

        [Fact]
        public void AddSchema_ToGroupWithoutSchemas_ShouldAddSchemaWithInitialVersion()
        {
            var schemaGroup = new ProtoBufSchemaGroup(this.fixture.Create<string>());

            schemaGroup.AddSchema(
                this.validSchema,
                new GroupConfiguration(true, true, true, true),
                new Dictionary<RuleCode, RuleConfiguration>());

            Assert.NotEmpty(schemaGroup.Schemas);
            Assert.Equal(Version.Initial, schemaGroup.Schemas[0].Version);
        }

        [Fact]
        public void AddSchema_BackwardCompatibleWithLastSchema_ShouldReturnNoRuleViolations()
        {
            var groupId = this.fixture.Create<long>();
            var firstSchema = new ProtoBufSchema(this.fixture.Create<long>(), Version.Initial, this.schemaWithNewMessage);
            var schemaGroup = new ProtoBufSchemaGroup(
                groupId,
                this.fixture.Create<string>(),
                new List<ProtoBufSchema> { firstSchema });

            IEnumerable<RuleViolation> ruleViolations = schemaGroup.AddSchema(
                this.baseSchema,
                new GroupConfiguration(false, true, false, false),
                this.CreateRulesConfiguration(Severity.Error));

            Assert.Empty(ruleViolations);
        }

        [Fact]
        public void AddSchema_ForwardCompatibleWithLastSchema_ShouldReturnNoRuleViolations()
        {
            var groupId = this.fixture.Create<long>();
            var firstSchema = new ProtoBufSchema(this.fixture.Create<long>(), Version.Initial, this.baseSchema);
            var schemaGroup = new ProtoBufSchemaGroup(
                groupId,
                this.fixture.Create<string>(),
                new List<ProtoBufSchema> { firstSchema });

            IEnumerable<RuleViolation> ruleViolations = schemaGroup.AddSchema(
                this.schemaWithNewMessage,
                new GroupConfiguration(true, false, false, false),
                this.CreateRulesConfiguration(Severity.Error));

            Assert.Empty(ruleViolations);
        }

        [Fact]
        public void AddSchema_TransitivelyBackwardCompatibleWithOlderSchemas_ShouldReturnNoRuleViolations()
        {
            var groupId = this.fixture.Create<long>();
            var firstSchema = new ProtoBufSchema(this.fixture.Create<long>(), Version.Initial, this.schemaWithNewMessage);
            var previousSchema = new ProtoBufSchema(this.fixture.Create<long>(), Version.Initial.Next(), this.baseSchema);
            var schemaGroup = new ProtoBufSchemaGroup(
                groupId,
                this.fixture.Create<string>(),
                new List<ProtoBufSchema> { firstSchema, previousSchema });

            IEnumerable<RuleViolation> ruleViolations = schemaGroup.AddSchema(
                this.schemaWithRemovedMessage,
                new GroupConfiguration(false, true, true, false),
                this.CreateRulesConfiguration(Severity.Error));

            Assert.Empty(ruleViolations);
        }

        [Fact]
        public void AddSchema_TransitivelyBackwardIncompatibleWithOlderSchemas_ShouldReturnRuleViolations()
        {
            var groupId = this.fixture.Create<long>();
            var firstSchema = new ProtoBufSchema(this.fixture.Create<long>(), Version.Initial, this.schemaWithRemovedMessage);
            var previousSchema = new ProtoBufSchema(this.fixture.Create<long>(), Version.Initial.Next(), this.schemaWithNewMessage);
            var schemaGroup = new ProtoBufSchemaGroup(
                groupId,
                this.fixture.Create<string>(),
                new List<ProtoBufSchema> { firstSchema, previousSchema });

            IEnumerable<RuleViolation> ruleViolations = schemaGroup.AddSchema(
                this.baseSchema,
                new GroupConfiguration(false, true, true, false),
                this.CreateRulesConfiguration(Severity.Error));

            Assert.NotEmpty(ruleViolations);
        }

        [Fact]
        public void AddSchema_TransitivelyForwardCompatibleWithOlderSchemas_ShouldReturnNoRuleViolations()
        {
            var groupId = this.fixture.Create<long>();
            var firstSchema = new ProtoBufSchema(this.fixture.Create<long>(), Version.Initial, this.schemaWithRemovedMessage);
            var previousSchema = new ProtoBufSchema(this.fixture.Create<long>(), Version.Initial.Next(), this.baseSchema);
            var schemaGroup = new ProtoBufSchemaGroup(
                groupId,
                this.fixture.Create<string>(),
                new List<ProtoBufSchema> { firstSchema, previousSchema });

            IEnumerable<RuleViolation> ruleViolations = schemaGroup.AddSchema(
                this.schemaWithNewMessage,
                new GroupConfiguration(true, false, true, false),
                this.CreateRulesConfiguration(Severity.Error));

            Assert.Empty(ruleViolations);
        }

        [Fact]
        public void AddSchema_TransitivelyForwardIncompatibleWithOlderSchemas_ShouldReturnRuleViolations()
        {
            var groupId = this.fixture.Create<long>();
            var firstSchema = new ProtoBufSchema(this.fixture.Create<long>(), Version.Initial, this.schemaWithNewMessage);
            var previousSchema = new ProtoBufSchema(this.fixture.Create<long>(), Version.Initial.Next(), this.schemaWithRemovedMessage);
            var schemaGroup = new ProtoBufSchemaGroup(
                groupId,
                this.fixture.Create<string>(),
                new List<ProtoBufSchema> { firstSchema, previousSchema });

            IEnumerable<RuleViolation> ruleViolations = schemaGroup.AddSchema(
                this.baseSchema,
                new GroupConfiguration(true, false, true, false),
                this.CreateRulesConfiguration(Severity.Error));

            Assert.NotEmpty(ruleViolations);
        }

        [Fact]
        public void AddSchema_WithFatalRuleViolations_ShouldNotAddSchema()
        {
            var groupId = this.fixture.Create<long>();
            var firstSchema = new ProtoBufSchema(this.fixture.Create<long>(), Version.Initial, this.schemaWithNewMessage);
            var schemaGroup = new ProtoBufSchemaGroup(
                groupId,
                this.fixture.Create<string>(),
                new List<ProtoBufSchema> { firstSchema });

            IEnumerable<RuleViolation> ruleViolations = schemaGroup.AddSchema(
                this.baseSchema,
                new GroupConfiguration(true, false, false, false),
                this.CreateRulesConfiguration(Severity.Error));

            Assert.Contains(ruleViolations, r => r.Severity.IsFatal);
            Assert.Equal(1, schemaGroup.Schemas.Count);
        }

        [Fact]
        public void AddSchema_WithNonFatalRuleViolations_ShouldAddSchema()
        {
            var groupId = this.fixture.Create<long>();
            var firstSchema = new ProtoBufSchema(this.fixture.Create<long>(), Version.Initial, this.schemaWithNewMessage);
            var schemaGroup = new ProtoBufSchemaGroup(
                groupId,
                this.fixture.Create<string>(),
                new List<ProtoBufSchema> { firstSchema });

            IEnumerable<RuleViolation> ruleViolations = schemaGroup.AddSchema(
                this.baseSchema,
                new GroupConfiguration(true, false, false, false),
                this.CreateRulesConfiguration(Severity.Warning));

            Assert.NotEmpty(ruleViolations);
            Assert.Equal(2, schemaGroup.Schemas.Count);
        }

        [Fact]
        public void AddSchema_WithRulesWithHiddenSeverity_ShouldIgnoreHiddenRules()
        {
            var groupId = this.fixture.Create<long>();
            var firstSchema = new ProtoBufSchema(this.fixture.Create<long>(), Version.Initial, this.schemaWithNewMessage);
            var schemaGroup = new ProtoBufSchemaGroup(
                groupId,
                this.fixture.Create<string>(),
                new List<ProtoBufSchema> { firstSchema });

            IEnumerable<RuleViolation> ruleViolations = schemaGroup.AddSchema(
                this.baseSchema,
                new GroupConfiguration(true, false, false, false),
                this.CreateRulesConfiguration(Severity.Hidden));

            Assert.Empty(ruleViolations);
        }

        [Fact]
        public void AddSchema_WithForwardIncompatibleContentsAndDisabledForwardCompatibility_ShouldAddSchema()
        {
            var groupId = this.fixture.Create<long>();
            var firstSchema = new ProtoBufSchema(this.fixture.Create<long>(), Version.Initial, this.schemaWithNewMessage);
            var schemaGroup = new ProtoBufSchemaGroup(
                groupId,
                this.fixture.Create<string>(),
                new List<ProtoBufSchema> { firstSchema });

            schemaGroup.AddSchema(
                this.baseSchema,
                new GroupConfiguration(false, true, false, false),
                this.CreateRulesConfiguration(Severity.Error));

            Assert.Equal(2, schemaGroup.Schemas.Count);
        }

        [Fact]
        public void AddSchema_WithBackwardIncompatibleContentsAndDisabledBackwardCompatibility_ShouldAddSchema()
        {
            var groupId = this.fixture.Create<long>();
            var firstSchema = new ProtoBufSchema(this.fixture.Create<long>(), Version.Initial, this.baseSchema);
            var schemaGroup = new ProtoBufSchemaGroup(
                groupId,
                this.fixture.Create<string>(),
                new List<ProtoBufSchema> { firstSchema });

            schemaGroup.AddSchema(
                this.schemaWithNewMessage,
                new GroupConfiguration(true, false, false, false),
                this.CreateRulesConfiguration(Severity.Error));

            Assert.Equal(2, schemaGroup.Schemas.Count);
        }

        private Dictionary<RuleCode, RuleConfiguration> CreateRulesConfiguration(Severity severity)
        {
            var rulesConfiguration = new Dictionary<RuleCode, RuleConfiguration>(RuleFactory
                .GetProtoBufRules()
                .Select(r => new KeyValuePair<RuleCode, RuleConfiguration>(r.Code, new RuleConfiguration(false, Severity.Hidden))))
            {
                [RuleCode.PB0002] = new RuleConfiguration(false, severity),
            };

            return rulesConfiguration;
        }
    }
}
