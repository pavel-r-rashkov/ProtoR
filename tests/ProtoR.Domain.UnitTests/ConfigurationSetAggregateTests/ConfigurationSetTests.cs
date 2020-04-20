namespace ProtoR.Domain.UnitTests.ConfigurationSetAggregateTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using ProtoR.Domain.GlobalConfigurationAggregate;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using Xunit;

    public class ConfigurationSetTests
    {
        private readonly Fixture fixture;

        public ConfigurationSetTests()
        {
            this.fixture = new Fixture();
        }

        [Fact]
        public void ShouldInherit_WithDefaultConfigurationSet_ShouldNotBeSet()
        {
            var configuration = new ConfigurationSet(
                this.fixture.Create<long>(),
                new Dictionary<RuleCode, RuleConfig>(),
                default,
                false,
                true,
                true,
                true)
            {
                ShouldInherit = true,
            };

            Assert.False(configuration.ShouldInherit);
        }

        [Fact]
        public void ShouldInherit_WithNonGlobalConfigurationSet_ShouldBeSet()
        {
            var configuration = new ConfigurationSet(
                this.fixture.Create<long>(),
                new Dictionary<RuleCode, RuleConfig>(),
                this.fixture.Create<long>(),
                false,
                true,
                true,
                true)
            {
                ShouldInherit = true,
            };

            Assert.True(configuration.ShouldInherit);
        }

        [Fact]
        public void GetRulesConfiguration_ShouldReturnRulesConfiguration()
        {
            var ruleConfig = new Dictionary<RuleCode, RuleConfig>()
            {
                { RuleCode.PB0001, new RuleConfig(false, Severity.Error) },
            };
            var configuration = new ConfigurationSet(
                this.fixture.Create<long>(),
                ruleConfig,
                this.fixture.Create<long>(),
                false,
                true,
                true,
                true);

            IReadOnlyDictionary<RuleCode, RuleConfig> result = configuration.GetRulesConfiguration();

            RuleCode expectedKey = ruleConfig.Keys.First();
            RuleCode actualKey = result.Keys.First();
            Assert.Equal(expectedKey, actualKey);
            Assert.Equal(ruleConfig[expectedKey], result[actualKey]);
        }

        [Fact]
        public void SetRulesConfiguration_WithNullArgument_ShouldThrow()
        {
            var configuration = new ConfigurationSet(
                this.fixture.Create<long>(),
                new Dictionary<RuleCode, RuleConfig>(),
                this.fixture.Create<long>(),
                false,
                true,
                true,
                true);

            Assert.Throws<ArgumentNullException>(() => configuration.SetRulesConfiguration(null));
        }

        [Fact]
        public void SetRulesConfiguration_WithNonGlobalConfigurationSet_ShouldSetInheritProperty()
        {
            var configuration = new ConfigurationSet(
                this.fixture.Create<long>(),
                new Dictionary<RuleCode, RuleConfig>(),
                this.fixture.Create<long>(),
                false,
                true,
                true,
                true);

            configuration.SetRulesConfiguration(new Dictionary<RuleCode, RuleConfig>()
            {
                { RuleCode.PB0001, new RuleConfig(true, Severity.Error) },
            });

            IReadOnlyDictionary<RuleCode, RuleConfig> rulesConfig = configuration.GetRulesConfiguration();
            Assert.True(rulesConfig[0].ShouldInherit);
        }

        [Fact]
        public void SetRulesConfiguration_WithGlobalConfigurationSet_ShouldSetInheritPropertyToFalse()
        {
            var configuration = new ConfigurationSet(
                this.fixture.Create<long>(),
                new Dictionary<RuleCode, RuleConfig>(),
                default,
                false,
                true,
                true,
                true);

            configuration.SetRulesConfiguration(new Dictionary<RuleCode, RuleConfig>
            {
                { RuleCode.PB0001, new RuleConfig(true, Severity.Error) },
            });

            IReadOnlyDictionary<RuleCode, RuleConfig> rulesConfig = configuration.GetRulesConfiguration();
            Assert.False(rulesConfig[0].ShouldInherit);
        }

        [Fact]
        public void SetCompatibility_WithNeitherForwardNorBackward_ShouldThrow()
        {
            var configuration = new ConfigurationSet(
                this.fixture.Create<long>(),
                new Dictionary<RuleCode, RuleConfig>(),
                default,
                false,
                true,
                true,
                true);

            Assert.Throws<ArgumentException>(() => configuration.SetCompatibility(false, false));
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void SetCompatibility_WithForwardOrBackwardOrBoth_ShouldSetBoth(
            bool backwardCompatible,
            bool forwardCompatible)
        {
            var configuration = new ConfigurationSet(
                this.fixture.Create<long>(),
                new Dictionary<RuleCode, RuleConfig>(),
                default,
                false,
                true,
                true,
                true);

            configuration.SetCompatibility(backwardCompatible, forwardCompatible);

            Assert.Equal(backwardCompatible, configuration.BackwardCompatible);
            Assert.Equal(forwardCompatible, configuration.ForwardCompatible);
        }
    }
}
