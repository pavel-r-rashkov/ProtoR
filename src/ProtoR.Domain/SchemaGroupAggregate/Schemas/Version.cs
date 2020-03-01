namespace ProtoR.Domain.SchemaGroupAggregate.Schemas
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using ProtoR.Domain.SeedWork;

    public class Version : ValueObject<Version>, IComparable<Version>
    {
        public static readonly Version Initial = new Version(1);

        public Version(int versionNumber)
        {
            this.VersionNumber = versionNumber;
        }

        public int VersionNumber { get; }

        public static bool operator ==(Version left, Version right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }

        public static bool operator !=(Version left, Version right)
        {
            return !(left == right);
        }

        public static bool operator <(Version left, Version right)
        {
            return left is null ? right is object : left.CompareTo(right) < 0;
        }

        public static bool operator <=(Version left, Version right)
        {
            return left is null || left.CompareTo(right) <= 0;
        }

        public static bool operator >(Version left, Version right)
        {
            return left is object && left.CompareTo(right) > 0;
        }

        public static bool operator >=(Version left, Version right)
        {
            return left is null ? right is null : left.CompareTo(right) >= 0;
        }

        public int CompareTo(Version other)
        {
            return this.VersionNumber.CompareTo(other.VersionNumber);
        }

        public Version Next()
        {
            return new Version(this.VersionNumber + 1);
        }

        public override string ToString()
        {
            return this.VersionNumber.ToString(CultureInfo.InvariantCulture);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is null)
            {
                return false;
            }

            var versionObject = obj as Version;

            if (versionObject == null)
            {
                return false;
            }

            return this.VersionNumber.Equals(versionObject.VersionNumber);
        }

        public override int GetHashCode()
        {
            return this.VersionNumber.GetHashCode();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.VersionNumber;
        }
    }
}
