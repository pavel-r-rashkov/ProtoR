namespace ProtoR.Domain.UnitTests.ConfigurationAggregateTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using Xunit;

    public class ConfigurationTests
    {
        private readonly Fixture fixture;

        public ConfigurationTests()
        {
            this.fixture = new Fixture();
        }

        [Fact]
        public void Inherit_WithDefaultConfiguration_ShouldNotBeSet()
        {
            var configuration = new Configuration(
                this.fixture.Create<long>(),
                new Dictionary<RuleCode, RuleConfiguration>(),
                null,
                new GroupConfiguration(false, true, false, true));

            Assert.False(configuration.GroupConfiguration.Inherit);
        }

        [Fact]
        public void Inherit_WithNonGlobalConfiguration_ShouldBeSet()
        {
            var configuration = new Configuration(
                this.fixture.Create<long>(),
                new Dictionary<RuleCode, RuleConfiguration>(),
                this.fixture.Create<long>(),
                new GroupConfiguration(false, true, false, true));

            Assert.True(configuration.GroupConfiguration.Inherit);
        }

        [Fact]
        public void GetRulesConfiguration_ShouldReturnRulesConfiguration()
        {
            var ruleConfig = new Dictionary<RuleCode, RuleConfiguration>()
            {
                { RuleCode.PB0001, new RuleConfiguration(false, Severity.Error) },
            };
            var configuration = new Configuration(
                this.fixture.Create<long>(),
                ruleConfig,
                this.fixture.Create<long>(),
                new GroupConfiguration(false, true, false, true));

            IReadOnlyDictionary<RuleCode, RuleConfiguration> result = configuration.GetRulesConfiguration();

            RuleCode expectedKey = ruleConfig.Keys.First();
            RuleCode actualKey = result.Keys.First();
            Assert.Equal(expectedKey, actualKey);
            Assert.Equal(ruleConfig[expectedKey], result[actualKey]);
        }

        [Fact]
        public void SetRulesConfiguration_WithNullArgument_ShouldThrow()
        {
            var configuration = new Configuration(
                this.fixture.Create<long>(),
                new Dictionary<RuleCode, RuleConfiguration>(),
                this.fixture.Create<long>(),
                new GroupConfiguration(false, true, false, true));

            Assert.Throws<ArgumentNullException>(() => configuration.SetRulesConfiguration(null));
        }

        [Fact]
        public void SetRulesConfiguration_WithNonGlobalConfiguration_ShouldSetInheritProperty()
        {
            var configuration = new Configuration(
                this.fixture.Create<long>(),
                new Dictionary<RuleCode, RuleConfiguration>(),
                this.fixture.Create<long>(),
                new GroupConfiguration(false, true, false, true));

            configuration.SetRulesConfiguration(new Dictionary<RuleCode, RuleConfiguration>()
            {
                { RuleCode.PB0001, new RuleConfiguration(true, Severity.Error) },
            });

            IReadOnlyDictionary<RuleCode, RuleConfiguration> rulesConfig = configuration.GetRulesConfiguration();
            Assert.True(rulesConfig[0].Inherit);
        }

        [Fact]
        public void SetRulesConfiguration_WithGlobalConfiguration_ShouldSetInheritPropertyToFalse()
        {
            var configuration = new Configuration(
                this.fixture.Create<long>(),
                new Dictionary<RuleCode, RuleConfiguration>(),
                default,
                new GroupConfiguration(false, true, false, true));

            configuration.SetRulesConfiguration(new Dictionary<RuleCode, RuleConfiguration>
            {
                { RuleCode.PB0001, new RuleConfiguration(true, Severity.Error) },
            });

            IReadOnlyDictionary<RuleCode, RuleConfiguration> rulesConfig = configuration.GetRulesConfiguration();
            Assert.False(rulesConfig[0].Inherit);
        }

        [Fact]
        public void SetGroupConfiguration_WithNullValue_ShouldThrow()
        {
            var configuration = new Configuration(
                this.fixture.Create<long>(),
                new Dictionary<RuleCode, RuleConfiguration>(),
                default,
                new GroupConfiguration(false, true, false, false));

            Assert.Throws<ArgumentNullException>(() => configuration.SetGroupConfiguration(null));
        }

        [Fact]
        public void SetGroupConfiguration_ForGlobalConfiguration_ShouldSetInheritToFalse()
        {
            var configuration = new Configuration(
                this.fixture.Create<long>(),
                new Dictionary<RuleCode, RuleConfiguration>(),
                null,
                new GroupConfiguration(false, true, false, false));

            configuration.SetGroupConfiguration(new GroupConfiguration(false, true, false, true));

            Assert.False(configuration.GroupConfiguration.Inherit);
        }

        [Fact]
        public void SetGroupConfiguration_ForNonGlobalConfiguration_ShouldSetGroupConfiguration()
        {
            var configuration = new Configuration(
                this.fixture.Create<long>(),
                new Dictionary<RuleCode, RuleConfiguration>(),
                1,
                new GroupConfiguration(false, true, false, false));
            var groupConfiguration = new GroupConfiguration(false, true, false, true);

            configuration.SetGroupConfiguration(groupConfiguration);

            Assert.Equal(groupConfiguration, configuration.GroupConfiguration);
        }

        [Fact]
        public void MergeRuleConfiguration_WithNull_ShoudThrow()
        {
            var configuration = new Configuration(
                this.fixture.Create<long>(),
                new Dictionary<RuleCode, RuleConfiguration>(),
                1,
                new GroupConfiguration(false, true, false, false));

            Assert.Throws<ArgumentNullException>(() => configuration.MergeRuleConfiguration(null));
        }

        [Fact]
        public void MergeRuleConfiguration_ShoudOverrideInheritedConfigurations()
        {
            var configuration = new Configuration(
                this.fixture.Create<long>(),
                new Dictionary<RuleCode, RuleConfiguration>()
                {
                    { RuleCode.PB0001, new RuleConfiguration(true, Severity.Hidden) },
                    { RuleCode.PB0002, new RuleConfiguration(false, Severity.Hidden) },
                },
                1,
                new GroupConfiguration(false, true, false, false));

            var globalConfiguration = new Configuration(
                this.fixture.Create<long>(),
                new Dictionary<RuleCode, RuleConfiguration>()
                {
                    { RuleCode.PB0001, new RuleConfiguration(false, Severity.Error) },
                    { RuleCode.PB0002, new RuleConfiguration(false, Severity.Error) },
                },
                null,
                new GroupConfiguration(false, true, false, false));

            IReadOnlyDictionary<RuleCode, RuleConfiguration> rulesConfiguration = configuration.MergeRuleConfiguration(globalConfiguration);

            Assert.Equal(rulesConfiguration[RuleCode.PB0001].Severity, Severity.Error);
            Assert.Equal(rulesConfiguration[RuleCode.PB0002].Severity, Severity.Hidden);
        }
    }
}
