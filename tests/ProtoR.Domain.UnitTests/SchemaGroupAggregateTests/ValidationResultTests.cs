namespace ProtoR.Domain.UnitTests.SchemaGroupAggregateTests
{
    using ProtoR.Domain.SchemaGroupAggregate;
    using Xunit;

    public class ValidationResultTests
    {
        [Fact]
        public void ValidationResult_ShouldBeCreated()
        {
            var validationResult = new ValidationResult(true, "description");

            Assert.NotNull(validationResult);
        }

        [Fact]
        public void Equality_WithEqualProperties_ShouldBeTrue()
        {
            var firstValidationResult = new ValidationResult(true, "description");
            var secondValidationResult = new ValidationResult(true, "description");

            Assert.Equal(firstValidationResult, secondValidationResult);
        }

        [Fact]
        public void EqualityOperator_WithEqualValidationResults_ShouldBeTrue()
        {
            var firstValidationResult = new ValidationResult(true, "description");
            var secondValidationResult = new ValidationResult(true, "description");

            Assert.True(firstValidationResult == secondValidationResult);
        }

        [Fact]
        public void EqualityOperator_WithNullValidationResults_ShouldBeTrue()
        {
            ValidationResult firstValidationResult = null;
            ValidationResult secondValidationResult = null;

            Assert.True(firstValidationResult == secondValidationResult);
        }

        [Fact]
        public void EqualityOperator_WithExactlyOneValueNull_ShouldBeFalse()
        {
            ValidationResult firstValidationResult = new ValidationResult(true, "description");
            ValidationResult secondValidationResult = null;

            Assert.False(firstValidationResult == secondValidationResult);
        }

        [Fact]
        public void InequalityOperator_WithDifferentValidationResults_ShouldBeTrue()
        {
            var firstValidationResult = new ValidationResult(true, "description");
            var secondValidationResult = new ValidationResult(false, "description");

            Assert.True(firstValidationResult != secondValidationResult);
        }

        [Fact]
        public void GetHashCode_WithEqualValidationResults_ShouldBeEqual()
        {
            var firstValidationResult = new ValidationResult(true, "description");
            var secondValidationResult = new ValidationResult(true, "description");

            Assert.True(firstValidationResult.GetHashCode() == secondValidationResult.GetHashCode());
        }
    }
}
