namespace ProtoR.Domain.SeedWork
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class ValueObject<T> : IEquatable<T>
        where T : ValueObject<T>
    {
        public static bool operator ==(ValueObject<T> a, ValueObject<T> b)
        {
            if (a is null && b is null)
            {
                return true;
            }

            if (a is null || b is null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(ValueObject<T> a, ValueObject<T> b)
        {
            return !(a == b);
        }

        public bool Equals(T other)
        {
            return this.Equals(other);
        }

        public override bool Equals(object obj)
        {
            var valueObject = obj as T;

            if (valueObject == null)
            {
                return false;
            }

            if (this.GetType() != obj.GetType())
            {
                return false;
            }

            return this.GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            return this.GetEqualityComponents()
                .Aggregate(1, (current, obj) =>
                {
                    unchecked
                    {
                        return current * 23 + (obj?.GetHashCode() ?? 0);
                    }
                });
        }

        protected abstract IEnumerable<object> GetEqualityComponents();
    }
}
