namespace ProtoR.Domain.SchemaGroupAggregate
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using ProtoR.Domain.SeedWork;

    public class GroupRestriction : ValueObject<GroupRestriction>
    {
        public const string WildCard = "*";
        private static readonly Regex PatternValidator = new Regex(@"^([\s0-9a-zA-Z\-_\*]+)$");

        public GroupRestriction(string pattern)
        {
            this.ValidatePattern(pattern);
            this.Pattern = pattern;
        }

        public string Pattern { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return this.Pattern;
        }

        private void ValidatePattern(string pattern)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            if (!PatternValidator.IsMatch(pattern))
            {
                throw new ArgumentException("Pattern can contain alphanumeric characters, _, -, or *", nameof(pattern));
            }
        }
    }
}
