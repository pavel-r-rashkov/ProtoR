namespace ProtoR.Domain.SchemaGroupAggregate.Rules
{
    using System.Collections.Generic;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.SeedWork;

    public class RuleViolation : ValueObject<RuleViolation>
    {
        public RuleViolation(
            ValidationResult validationResult,
            Severity severity,
            Version newVersion,
            Version oldVersion,
            bool backwardCompatibilityViolation)
        {
            this.ValidationResult = validationResult;
            this.Severity = severity;
            this.NewVersion = newVersion;
            this.OldVersion = oldVersion;
            this.BackwardCompatibilityViolation = backwardCompatibilityViolation;
        }

        public ValidationResult ValidationResult { get; }

        public Severity Severity { get; }

        public Version NewVersion { get; }

        public Version OldVersion { get; }

        public bool BackwardCompatibilityViolation { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.ValidationResult;
            yield return this.Severity;
            yield return this.NewVersion;
            yield return this.OldVersion;
            yield return this.BackwardCompatibilityViolation;
        }
    }
}
