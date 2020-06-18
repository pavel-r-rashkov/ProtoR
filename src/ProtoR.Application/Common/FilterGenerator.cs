namespace ProtoR.Application.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ProtoR.Domain.SchemaGroupAggregate;

    public static class FilterGenerator
    {
        public static string CreateFromPatterns(IEnumerable<string> patterns)
        {
            var regexPatterns = patterns.Select(p =>
            {
                var patternRegex = p.Replace(GroupRestriction.WildCard, @"[\s\S]*", StringComparison.InvariantCulture);
                return $"({patternRegex})";
            });

            return $"^{string.Join('|', regexPatterns)}$";
        }
    }
}
