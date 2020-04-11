namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
    using Google.Protobuf.Reflection;
    using Moq;
    using ProtoR.Domain.GlobalConfigurationAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using Xunit;
    using Version = ProtoR.Domain.SchemaGroupAggregate.Schemas.Version;

    public class SchemaGroupTests
    {
        private readonly Mock<ISchemaFactory<ProtoBufSchema, FileDescriptorSet>> schemaFactoryMock;
        private readonly Mock<ProtoBufRule> ruleMock;
        private readonly Fixture fixture;

        public SchemaGroupTests()
        {
            this.schemaFactoryMock = new Mock<ISchemaFactory<ProtoBufSchema, FileDescriptorSet>>();
            this.ruleMock = new Mock<ProtoBufRule>(RuleCode.R0001);
            this.fixture = new Fixture();
        }

        [Fact]
        public void Name_ShouldReturnSchemaGroupName()
        {
            var name = this.fixture.Create<string>();
            var schemaGroup = new SchemaGroup<ProtoBufSchema, FileDescriptorSet>(name);

            Assert.Equal(name, schemaGroup.Name);
        }

        [Fact]
        public void AddSchema_WithNullConfigurationSet_ShouldThrow()
        {
            var schemaGroup = this.fixture.Create<SchemaGroup<ProtoBufSchema, FileDescriptorSet>>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                schemaGroup.AddSchema(string.Empty, null, this.schemaFactoryMock.Object);
            });
        }

        [Fact]
        public void AddSchema_WithNullSchemaFactory_ShouldThrow()
        {
            var schemaGroup = this.fixture.Create<SchemaGroup<ProtoBufSchema, FileDescriptorSet>>();

            Assert.Throws<ArgumentNullException>(() =>
            {
                schemaGroup.AddSchema(
                    string.Empty,
                    this.fixture.Create<ConfigurationSet>(),
                    null);
            });
        }

        [Fact]
        public void AddSchema_WithConfigurationSetNotAssociatedWithTheSchemaGroup_ShouldThrow()
        {
            var schemaGroup = this.fixture.Create<SchemaGroup<ProtoBufSchema, FileDescriptorSet>>();
            var config = new ConfigurationSet(
                Guid.NewGuid(),
                new Dictionary<RuleCode, RuleConfig>(),
                Guid.NewGuid(),
                true,
                true,
                true,
                true);

            Assert.Throws<ArgumentException>(() =>
            {
                schemaGroup.AddSchema(
                    string.Empty,
                    this.fixture.Create<ConfigurationSet>(),
                    this.schemaFactoryMock.Object);
            });
        }

        [Fact]
        public void AddSchema_WithInvalidContents_ShouldThrow()
        {
            this.schemaFactoryMock
                .Setup(factory => factory.CreateNew(It.IsAny<Version>(), It.IsAny<string>()))
                .Throws<InvalidProtoBufSchemaException>();
            var schemaGroup = this.fixture.Create<SchemaGroup<ProtoBufSchema, FileDescriptorSet>>();
            var config = new ConfigurationSet(
                Guid.NewGuid(),
                new Dictionary<RuleCode, RuleConfig>(),
                schemaGroup.Id,
                true,
                true,
                true,
                true);

            Assert.Throws<InvalidProtoBufSchemaException>(() =>
            {
                schemaGroup.AddSchema(
                    string.Empty,
                    config,
                    this.schemaFactoryMock.Object);
            });
        }

        [Fact]
        public void AddSchema_ToGroupWithoutSchemas_ShouldAddSchemaWithInitialVersion()
        {
            var schemaGroup = new SchemaGroup<ProtoBufSchema, FileDescriptorSet>(this.fixture.Create<string>());
            var contents = this.fixture.Create<string>();
            var config = new ConfigurationSet(
                Guid.NewGuid(),
                new Dictionary<RuleCode, RuleConfig>(),
                schemaGroup.Id,
                true,
                true,
                true,
                true);

            schemaGroup.AddSchema(
                contents,
                config,
                this.schemaFactoryMock.Object);

            this.schemaFactoryMock.Verify(
                factory => factory.CreateNew(Version.Initial, contents),
                "New schema should have initial version when the schema group doesn't have any schemas");
        }

        [Fact]
        public void AddSchema_BackwardCompatibleWithLastSchema_ShouldReturnNoRuleViolations()
        {
            this.ruleMock
                .Setup(rule => rule.Validate(It.IsAny<ProtoBufSchema>(), It.Is<ProtoBufSchema>(schema => schema.Version == Version.Initial.Next())))
                .Returns(new ValidationResult(true, this.fixture.Create<string>()));

            this.ruleMock
                .Setup(rule => rule.Validate(It.IsAny<ProtoBufSchema>(), It.Is<ProtoBufSchema>(schema => schema.Version == Version.Initial)))
                .Returns(new ValidationResult(false, this.fixture.Create<string>()));

            this.schemaFactoryMock
                .Setup(factory => factory.CreateNew(It.IsAny<Version>(), It.IsAny<string>()))
                .Returns(new ProtoBufSchema(Guid.NewGuid(), Version.Initial.Next().Next(), string.Empty));

            var groupId = this.fixture.Create<Guid>();
            var firstSchema = new ProtoBufSchema(Guid.NewGuid(), Version.Initial, string.Empty);
            var previousSchema = new ProtoBufSchema(Guid.NewGuid(), Version.Initial.Next(), string.Empty);
            var schemaGroup = new SchemaGroup<ProtoBufSchema, FileDescriptorSet>(
                groupId,
                this.fixture.Create<string>(),
                new List<ProtoBufSchema> { firstSchema, previousSchema },
                new List<ProtoBufRule> { this.ruleMock.Object });

            var config = new ConfigurationSet(
                this.fixture.Create<Guid>(),
                new Dictionary<RuleCode, RuleConfig>
                {
                    { RuleCode.R0001, new RuleConfig(false, Severity.Error) },
                },
                groupId,
                false,
                false,
                true,
                false);

            IEnumerable<RuleViolation> ruleViolations = schemaGroup.AddSchema(
                this.fixture.Create<string>(),
                config,
                this.schemaFactoryMock.Object);

            Assert.Empty(ruleViolations);
        }

        [Fact]
        public void AddSchema_ForwardCompatibleWithLastSchema_ShouldReturnNoRuleViolations()
        {
            this.ruleMock
                .Setup(rule => rule.Validate(It.Is<ProtoBufSchema>(schema => schema.Version == Version.Initial.Next()), It.IsAny<ProtoBufSchema>()))
                .Returns(new ValidationResult(true, this.fixture.Create<string>()));

            this.ruleMock
                .Setup(rule => rule.Validate(It.Is<ProtoBufSchema>(schema => schema.Version == Version.Initial), It.IsAny<ProtoBufSchema>()))
                .Returns(new ValidationResult(false, this.fixture.Create<string>()));

            this.schemaFactoryMock
                .Setup(factory => factory.CreateNew(It.IsAny<Version>(), It.IsAny<string>()))
                .Returns(new ProtoBufSchema(Guid.NewGuid(), Version.Initial.Next().Next(), string.Empty));

            var groupId = this.fixture.Create<Guid>();
            var firstSchema = new ProtoBufSchema(Guid.NewGuid(), Version.Initial, string.Empty);
            var previousSchema = new ProtoBufSchema(Guid.NewGuid(), Version.Initial.Next(), string.Empty);
            var schemaGroup = new SchemaGroup<ProtoBufSchema, FileDescriptorSet>(
                groupId,
                this.fixture.Create<string>(),
                new List<ProtoBufSchema> { firstSchema, previousSchema },
                new List<ProtoBufRule> { this.ruleMock.Object });

            var config = new ConfigurationSet(
                this.fixture.Create<Guid>(),
                new Dictionary<RuleCode, RuleConfig>
                {
                    { RuleCode.R0001, new RuleConfig(false, Severity.Error) },
                },
                groupId,
                false,
                true,
                false,
                false);

            IEnumerable<RuleViolation> ruleViolations = schemaGroup.AddSchema(
                this.fixture.Create<string>(),
                config,
                this.schemaFactoryMock.Object);

            Assert.Empty(ruleViolations);
        }

        [Fact]
        public void AddSchema_TransitivelyBackwardCompatibleWithOlderSchemas_ShouldReturnNoRuleViolations()
        {
            this.ruleMock
                .Setup(rule => rule.Validate(It.IsAny<ProtoBufSchema>(), It.Is<ProtoBufSchema>(schema => schema.Version == Version.Initial.Next())))
                .Returns(new ValidationResult(true, this.fixture.Create<string>()));

            this.ruleMock
                .Setup(rule => rule.Validate(It.IsAny<ProtoBufSchema>(), It.Is<ProtoBufSchema>(schema => schema.Version == Version.Initial)))
                .Returns(new ValidationResult(true, this.fixture.Create<string>()));

            this.schemaFactoryMock
                .Setup(factory => factory.CreateNew(It.IsAny<Version>(), It.IsAny<string>()))
                .Returns(new ProtoBufSchema(Guid.NewGuid(), Version.Initial.Next().Next(), string.Empty));

            var groupId = this.fixture.Create<Guid>();
            var firstSchema = new ProtoBufSchema(Guid.NewGuid(), Version.Initial, string.Empty);
            var previousSchema = new ProtoBufSchema(Guid.NewGuid(), Version.Initial.Next(), string.Empty);
            var schemaGroup = new SchemaGroup<ProtoBufSchema, FileDescriptorSet>(
                groupId,
                this.fixture.Create<string>(),
                new List<ProtoBufSchema> { firstSchema, previousSchema },
                new List<ProtoBufRule> { this.ruleMock.Object });

            var config = new ConfigurationSet(
                this.fixture.Create<Guid>(),
                new Dictionary<RuleCode, RuleConfig>
                {
                    { RuleCode.R0001, new RuleConfig(false, Severity.Error) },
                },
                groupId,
                false,
                false,
                true,
                true);

            IEnumerable<RuleViolation> ruleViolations = schemaGroup.AddSchema(
                this.fixture.Create<string>(),
                config,
                this.schemaFactoryMock.Object);

            Assert.Empty(ruleViolations);
        }

        [Fact]
        public void AddSchema_TransitivelyBackwardIncompatibleWithOlderSchemas_ShouldReturnRuleViolations()
        {
            this.ruleMock
                .Setup(rule => rule.Validate(It.IsAny<ProtoBufSchema>(), It.Is<ProtoBufSchema>(schema => schema.Version == Version.Initial.Next())))
                .Returns(new ValidationResult(false, this.fixture.Create<string>()));

            this.ruleMock
                .Setup(rule => rule.Validate(It.IsAny<ProtoBufSchema>(), It.Is<ProtoBufSchema>(schema => schema.Version == Version.Initial)))
                .Returns(new ValidationResult(true, this.fixture.Create<string>()));

            this.schemaFactoryMock
                .Setup(factory => factory.CreateNew(It.IsAny<Version>(), It.IsAny<string>()))
                .Returns(new ProtoBufSchema(Guid.NewGuid(), Version.Initial.Next().Next(), string.Empty));

            var groupId = this.fixture.Create<Guid>();
            var firstSchema = new ProtoBufSchema(Guid.NewGuid(), Version.Initial, string.Empty);
            var previousSchema = new ProtoBufSchema(Guid.NewGuid(), Version.Initial.Next(), string.Empty);
            var schemaGroup = new SchemaGroup<ProtoBufSchema, FileDescriptorSet>(
                groupId,
                this.fixture.Create<string>(),
                new List<ProtoBufSchema> { firstSchema, previousSchema },
                new List<ProtoBufRule> { this.ruleMock.Object });

            var config = new ConfigurationSet(
                this.fixture.Create<Guid>(),
                new Dictionary<RuleCode, RuleConfig>
                {
                    { RuleCode.R0001, new RuleConfig(false, Severity.Error) },
                },
                groupId,
                false,
                false,
                true,
                true);

            IEnumerable<RuleViolation> ruleViolations = schemaGroup.AddSchema(
                this.fixture.Create<string>(),
                config,
                this.schemaFactoryMock.Object);

            Assert.NotEmpty(ruleViolations);
        }

        [Fact]
        public void AddSchema_TransitivelyForwardCompatibleWithOlderSchemas_ShouldReturnNoRuleViolations()
        {
            this.ruleMock
                .Setup(rule => rule.Validate(It.Is<ProtoBufSchema>(schema => schema.Version == Version.Initial.Next()), It.IsAny<ProtoBufSchema>()))
                .Returns(new ValidationResult(true, this.fixture.Create<string>()));

            this.ruleMock
                .Setup(rule => rule.Validate(It.Is<ProtoBufSchema>(schema => schema.Version == Version.Initial), It.IsAny<ProtoBufSchema>()))
                .Returns(new ValidationResult(true, this.fixture.Create<string>()));

            this.schemaFactoryMock
                .Setup(factory => factory.CreateNew(It.IsAny<Version>(), It.IsAny<string>()))
                .Returns(new ProtoBufSchema(Guid.NewGuid(), Version.Initial.Next().Next(), string.Empty));

            var groupId = this.fixture.Create<Guid>();
            var firstSchema = new ProtoBufSchema(Guid.NewGuid(), Version.Initial, string.Empty);
            var previousSchema = new ProtoBufSchema(Guid.NewGuid(), Version.Initial.Next(), string.Empty);
            var schemaGroup = new SchemaGroup<ProtoBufSchema, FileDescriptorSet>(
                groupId,
                this.fixture.Create<string>(),
                new List<ProtoBufSchema> { firstSchema, previousSchema },
                new List<ProtoBufRule> { this.ruleMock.Object });

            var config = new ConfigurationSet(
                this.fixture.Create<Guid>(),
                new Dictionary<RuleCode, RuleConfig>
                {
                    { RuleCode.R0001, new RuleConfig(false, Severity.Error) },
                },
                groupId,
                false,
                true,
                false,
                true);

            IEnumerable<RuleViolation> ruleViolations = schemaGroup.AddSchema(
                this.fixture.Create<string>(),
                config,
                this.schemaFactoryMock.Object);

            Assert.Empty(ruleViolations);
        }

        [Fact]
        public void AddSchema_TransitivelyForwardIncompatibleWithOlderSchemas_ShouldReturnRuleViolations()
        {
            this.ruleMock
                .Setup(rule => rule.Validate(It.Is<ProtoBufSchema>(schema => schema.Version == Version.Initial.Next()), It.IsAny<ProtoBufSchema>()))
                .Returns(new ValidationResult(true, this.fixture.Create<string>()));

            this.ruleMock
                .Setup(rule => rule.Validate(It.Is<ProtoBufSchema>(schema => schema.Version == Version.Initial), It.IsAny<ProtoBufSchema>()))
                .Returns(new ValidationResult(false, this.fixture.Create<string>()));

            this.schemaFactoryMock
                .Setup(factory => factory.CreateNew(It.IsAny<Version>(), It.IsAny<string>()))
                .Returns(new ProtoBufSchema(Guid.NewGuid(), Version.Initial.Next().Next(), string.Empty));

            var groupId = this.fixture.Create<Guid>();
            var firstSchema = new ProtoBufSchema(Guid.NewGuid(), Version.Initial, string.Empty);
            var previousSchema = new ProtoBufSchema(Guid.NewGuid(), Version.Initial.Next(), string.Empty);
            var schemaGroup = new SchemaGroup<ProtoBufSchema, FileDescriptorSet>(
                groupId,
                this.fixture.Create<string>(),
                new List<ProtoBufSchema> { firstSchema, previousSchema },
                new List<ProtoBufRule> { this.ruleMock.Object });

            var config = new ConfigurationSet(
                this.fixture.Create<Guid>(),
                new Dictionary<RuleCode, RuleConfig>
                {
                    { RuleCode.R0001, new RuleConfig(false, Severity.Error) },
                },
                groupId,
                false,
                true,
                false,
                true);

            IEnumerable<RuleViolation> ruleViolations = schemaGroup.AddSchema(
                this.fixture.Create<string>(),
                config,
                this.schemaFactoryMock.Object);

            Assert.NotEmpty(ruleViolations);
        }

        [Fact]
        public void AddSchema_WithFatalRuleViolations_ShouldNotAddSchema()
        {
            this.ruleMock
                .Setup(rule => rule.Validate(It.Is<ProtoBufSchema>(schema => schema.Version == Version.Initial), It.IsAny<ProtoBufSchema>()))
                .Returns(new ValidationResult(false, this.fixture.Create<string>()));

            this.schemaFactoryMock
                .Setup(factory => factory.CreateNew(It.IsAny<Version>(), It.IsAny<string>()))
                .Returns(new ProtoBufSchema(Guid.NewGuid(), Version.Initial.Next(), string.Empty));

            var groupId = this.fixture.Create<Guid>();
            var firstSchema = new ProtoBufSchema(Guid.NewGuid(), Version.Initial, string.Empty);
            var schemaGroup = new SchemaGroup<ProtoBufSchema, FileDescriptorSet>(
                groupId,
                this.fixture.Create<string>(),
                new List<ProtoBufSchema> { firstSchema },
                new List<ProtoBufRule> { this.ruleMock.Object });

            var config = new ConfigurationSet(
                this.fixture.Create<Guid>(),
                new Dictionary<RuleCode, RuleConfig>
                {
                    { RuleCode.R0001, new RuleConfig(false, Severity.Error) },
                },
                groupId,
                false,
                true,
                false,
                false);

            IEnumerable<RuleViolation> ruleViolations = schemaGroup.AddSchema(
                this.fixture.Create<string>(),
                config,
                this.schemaFactoryMock.Object);

            Assert.Equal(1, schemaGroup.Schemas.Count);
        }

        [Fact]
        public void AddSchema_WithNonFatalRuleViolations_ShouldAddSchema()
        {
            this.ruleMock
                .Setup(rule => rule.Validate(It.Is<ProtoBufSchema>(schema => schema.Version == Version.Initial), It.IsAny<ProtoBufSchema>()))
                .Returns(new ValidationResult(false, this.fixture.Create<string>()));

            this.schemaFactoryMock
                .Setup(factory => factory.CreateNew(It.IsAny<Version>(), It.IsAny<string>()))
                .Returns(new ProtoBufSchema(Guid.NewGuid(), Version.Initial.Next(), string.Empty));

            var groupId = this.fixture.Create<Guid>();
            var firstSchema = new ProtoBufSchema(Guid.NewGuid(), Version.Initial, string.Empty);
            var schemaGroup = new SchemaGroup<ProtoBufSchema, FileDescriptorSet>(
                groupId,
                this.fixture.Create<string>(),
                new List<ProtoBufSchema> { firstSchema },
                new List<ProtoBufRule> { this.ruleMock.Object });

            var config = new ConfigurationSet(
                this.fixture.Create<Guid>(),
                new Dictionary<RuleCode, RuleConfig>
                {
                    { RuleCode.R0001, new RuleConfig(false, Severity.Warning) },
                },
                groupId,
                false,
                true,
                false,
                false);

            IEnumerable<RuleViolation> ruleViolations = schemaGroup.AddSchema(
                this.fixture.Create<string>(),
                config,
                this.schemaFactoryMock.Object);

            Assert.Equal(2, schemaGroup.Schemas.Count);
        }

        [Fact]
        public void AddSchema_WithRulesWithHiddenSeverity_ShouldIgnoreHiddenRules()
        {
            this.ruleMock
                .Setup(rule => rule.Validate(It.Is<ProtoBufSchema>(schema => schema.Version == Version.Initial), It.IsAny<ProtoBufSchema>()))
                .Returns(new ValidationResult(false, this.fixture.Create<string>()));

            this.schemaFactoryMock
                .Setup(factory => factory.CreateNew(It.IsAny<Version>(), It.IsAny<string>()))
                .Returns(new ProtoBufSchema(Guid.NewGuid(), Version.Initial.Next(), string.Empty));

            var groupId = this.fixture.Create<Guid>();
            var firstSchema = new ProtoBufSchema(Guid.NewGuid(), Version.Initial, string.Empty);
            var schemaGroup = new SchemaGroup<ProtoBufSchema, FileDescriptorSet>(
                groupId,
                this.fixture.Create<string>(),
                new List<ProtoBufSchema> { firstSchema },
                new List<ProtoBufRule> { this.ruleMock.Object });

            var config = new ConfigurationSet(
                this.fixture.Create<Guid>(),
                new Dictionary<RuleCode, RuleConfig>
                {
                    { RuleCode.R0001, new RuleConfig(false, Severity.Hidden) },
                },
                groupId,
                false,
                true,
                false,
                false);

            IEnumerable<RuleViolation> ruleViolations = schemaGroup.AddSchema(
                this.fixture.Create<string>(),
                config,
                this.schemaFactoryMock.Object);

            Assert.Empty(ruleViolations);
        }

        [Fact]
        public void AddSchema_WithForwardIncompatibleContentsAndDisabledForwardCompatibility_ShouldAddSchema()
        {
            this.ruleMock
                .Setup(rule => rule.Validate(It.Is<ProtoBufSchema>(schema => schema.Version == Version.Initial), It.IsAny<ProtoBufSchema>()))
                .Returns(new ValidationResult(false, this.fixture.Create<string>()));

            this.ruleMock
                .Setup(rule => rule.Validate(It.IsAny<ProtoBufSchema>(), It.Is<ProtoBufSchema>(schema => schema.Version == Version.Initial)))
                .Returns(new ValidationResult(true, this.fixture.Create<string>()));

            this.schemaFactoryMock
                .Setup(factory => factory.CreateNew(It.IsAny<Version>(), It.IsAny<string>()))
                .Returns(new ProtoBufSchema(Guid.NewGuid(), Version.Initial.Next().Next(), string.Empty));

            var groupId = this.fixture.Create<Guid>();
            var firstSchema = new ProtoBufSchema(Guid.NewGuid(), Version.Initial, string.Empty);
            var schemaGroup = new SchemaGroup<ProtoBufSchema, FileDescriptorSet>(
                groupId,
                this.fixture.Create<string>(),
                new List<ProtoBufSchema> { firstSchema },
                new List<ProtoBufRule> { this.ruleMock.Object });

            var config = new ConfigurationSet(
                this.fixture.Create<Guid>(),
                new Dictionary<RuleCode, RuleConfig>
                {
                    { RuleCode.R0001, new RuleConfig(false, Severity.Error) },
                },
                groupId,
                false,
                false,
                true,
                false);

            schemaGroup.AddSchema(
                this.fixture.Create<string>(),
                config,
                this.schemaFactoryMock.Object);

            Assert.Equal(2, schemaGroup.Schemas.Count);
        }

        [Fact]
        public void AddSchema_WithBackwardIncompatibleContentsAndDisabledBackwardCompatibility_ShouldAddSchema()
        {
            this.ruleMock
                .Setup(rule => rule.Validate(It.IsAny<ProtoBufSchema>(), It.Is<ProtoBufSchema>(schema => schema.Version == Version.Initial)))
                .Returns(new ValidationResult(false, this.fixture.Create<string>()));

            this.ruleMock
                .Setup(rule => rule.Validate(It.Is<ProtoBufSchema>(schema => schema.Version == Version.Initial), It.IsAny<ProtoBufSchema>()))
                .Returns(new ValidationResult(true, this.fixture.Create<string>()));

            this.schemaFactoryMock
                .Setup(factory => factory.CreateNew(It.IsAny<Version>(), It.IsAny<string>()))
                .Returns(new ProtoBufSchema(Guid.NewGuid(), Version.Initial.Next().Next(), string.Empty));

            var groupId = this.fixture.Create<Guid>();
            var firstSchema = new ProtoBufSchema(Guid.NewGuid(), Version.Initial, string.Empty);
            var schemaGroup = new SchemaGroup<ProtoBufSchema, FileDescriptorSet>(
                groupId,
                this.fixture.Create<string>(),
                new List<ProtoBufSchema> { firstSchema },
                new List<ProtoBufRule> { this.ruleMock.Object });

            var config = new ConfigurationSet(
                this.fixture.Create<Guid>(),
                new Dictionary<RuleCode, RuleConfig>
                {
                    { RuleCode.R0001, new RuleConfig(false, Severity.Error) },
                },
                groupId,
                false,
                true,
                false,
                false);

            schemaGroup.AddSchema(
                this.fixture.Create<string>(),
                config,
                this.schemaFactoryMock.Object);

            Assert.Equal(2, schemaGroup.Schemas.Count);
        }
    }
}
