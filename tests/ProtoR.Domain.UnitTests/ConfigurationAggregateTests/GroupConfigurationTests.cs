namespace ProtoR.Domain.UnitTests.ConfigurationAggregateTests
{
    using System;
    using ProtoR.Domain.ConfigurationAggregate;
    using Xunit;

    public class GroupConfigurationTests
    {
        [Fact]
        public void Equals_WithEqualConfigurations_ShouldReturnTrue()
        {
            var configurationA = new GroupConfiguration(false, true, false, true);
            var configurationB = new GroupConfiguration(false, true, false, true);

            Assert.Equal(configurationB, configurationA);
        }

        [Fact]
        public void Equals_WithDifferentConfigurations_ShouldReturnFalse()
        {
            var configurationA = new GroupConfiguration(false, true, false, true);
            var configurationB = new GroupConfiguration(true, false, false, true);

            Assert.NotEqual(configurationB, configurationA);
        }

        [Fact]
        public void WithInheritance_ShouldReturnCopyWithInheritanceSet()
        {
            var configuration = new GroupConfiguration(false, true, false, true);

            GroupConfiguration result = configuration.WithInheritance(false);

            Assert.Equal(configuration.ForwardCompatible, result.ForwardCompatible);
            Assert.Equal(configuration.BackwardCompatible, result.BackwardCompatible);
            Assert.Equal(configuration.Transitive, result.Transitive);
            Assert.False(result.Inherit);
        }

        [Fact]
        public void GroupConfiguration_WithNeitherForwardNorBackwardCompatibility_ShouldThrow()
        {
            Assert.Throws<ArgumentException>(() => new GroupConfiguration(false, false, false, true));
        }
    }
}
