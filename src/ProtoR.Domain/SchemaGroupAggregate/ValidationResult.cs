namespace ProtoR.Domain.SchemaGroupAggregate
{
    using System.Collections.Generic;
    using ProtoR.Domain.SeedWork;

    public class ValidationResult : ValueObject<ValidationResult>
    {
        public ValidationResult(bool passed, string description)
        {
            this.Passed = passed;
            this.Description = description;
        }

        public bool Passed { get; }

        public string Description { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Passed;
            yield return this.Description;
        }
    }
}
