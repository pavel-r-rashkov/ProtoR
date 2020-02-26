namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests
{
    using System.Collections.Generic;
    using System.Linq;
    using ProtoR.Domain.SchemaGroupAggregate;
    using ProtoR.Domain.SeedWork;
    using Xunit;

    public class SeverityTests
    {
        [Fact]
        public void Severity_Order_ShouldBeFromLowerToHigher()
        {
            Assert.True(Severity.Hidden < Severity.Info);
            Assert.True(Severity.Info < Severity.Warning);
            Assert.True(Severity.Warning < Severity.Error);
        }

        [Fact]
        public void IsFatal_ForErrorSeverity_ShouldBeTrue()
        {
            Assert.True(Severity.Error.IsFatal);
        }

        [Theory]
        [MemberData(nameof(SeverityTests.NonFatalSeverities), MemberType = typeof(SeverityTests))]
        public void IsFatal_ForLowerThanErrorSeverity_ShouldBeFalse(Severity severity)
        {
            Assert.False(severity.IsFatal);
        }

        [Fact]
        public void Equality_WithSameSeverities_ShouldBeTrue()
        {
            Assert.True(Severity.Hidden == Severity.Hidden);
        }

        [Fact]
        public void Inequality_WithDifferentSeverities_ShouldBeTrue()
        {
            Assert.True(Severity.Hidden != Severity.Info);
        }

        [Theory]
        [MemberData(nameof(SeverityTests.LessThanOrEqualSeverities), MemberType = typeof(SeverityTests))]
        public void LessThanOrEqual_WithSmallerOrEqualSeverities_ShouldBeTrue(
            Severity firstSeverity,
            Severity secondSeverity)
        {
            Assert.True(firstSeverity <= secondSeverity);
        }

        [Theory]
        [MemberData(nameof(SeverityTests.GreaterThanOrEqualSeverities), MemberType = typeof(SeverityTests))]
        public void GreaterThanOrEqual_WithSmallerOrEqualSeverities_ShouldBeTrue(
            Severity firstSeverity,
            Severity secondSeverity)
        {
            Assert.True(firstSeverity >= secondSeverity);
        }

        [Fact]
        public void Severity_GetAll_ShouldReturnAllSeverities()
        {
            int severityCount = Enumeration.GetAll<Severity>().Count();

            Assert.Equal(4, severityCount);
        }

        public static IEnumerable<object[]> NonFatalSeverities()
        {
            return new List<object[]>
            {
                new object[] { Severity.Hidden },
                new object[] { Severity.Info },
                new object[] { Severity.Warning },
            };
        }

        public static IEnumerable<object[]> LessThanOrEqualSeverities()
        {
            return new List<object[]>
            {
                new object[] { Severity.Hidden, Severity.Hidden },
                new object[] { Severity.Info, Severity.Info },
                new object[] { Severity.Warning, Severity.Warning },
                new object[] { Severity.Error, Severity.Error },
                new object[] { Severity.Hidden, Severity.Info },
                new object[] { Severity.Info, Severity.Warning },
                new object[] { Severity.Warning, Severity.Error },
            };
        }

        public static IEnumerable<object[]> GreaterThanOrEqualSeverities()
        {
            return new List<object[]>
            {
                new object[] { Severity.Hidden, Severity.Hidden },
                new object[] { Severity.Info, Severity.Info },
                new object[] { Severity.Warning, Severity.Warning },
                new object[] { Severity.Error, Severity.Error },
                new object[] { Severity.Info, Severity.Hidden },
                new object[] { Severity.Warning, Severity.Info },
                new object[] { Severity.Error, Severity.Warning },
            };
        }
    }
}
