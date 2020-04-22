namespace ProtoR.Domain.ConfigurationAggregate
{
    using System;
    using System.Collections.Generic;
    using ProtoR.Domain.SeedWork;

    public class GroupConfiguration : ValueObject<GroupConfiguration>
    {
        public GroupConfiguration(
            bool forwardCompatible,
            bool backwardCompatible,
            bool transitive,
            bool inherit)
        {
            if (!forwardCompatible && !backwardCompatible)
            {
                throw new ArgumentException($"{nameof(forwardCompatible)} and {nameof(backwardCompatible)} cannot be both false");
            }

            this.ForwardCompatible = forwardCompatible;
            this.BackwardCompatible = backwardCompatible;
            this.Transitive = transitive;
            this.Inherit = inherit;
        }

        public bool ForwardCompatible { get; private set; }

        public bool BackwardCompatible { get; private set; }

        public bool Transitive { get; private set; }

        public bool Inherit { get; private set; }

        public GroupConfiguration WithInheritance(bool inherit)
        {
            return new GroupConfiguration(
                this.ForwardCompatible,
                this.BackwardCompatible,
                this.Transitive,
                inherit);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.ForwardCompatible;
            yield return this.BackwardCompatible;
            yield return this.Transitive;
            yield return this.Inherit;
        }
    }
}
