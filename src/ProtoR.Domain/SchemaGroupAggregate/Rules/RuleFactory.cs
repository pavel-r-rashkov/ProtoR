namespace ProtoR.Domain.SchemaGroupAggregate.Rules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ProtoR.Domain.SchemaGroupAggregate.Rules.ProtoBufRules;

    public static class RuleFactory
    {
        private static readonly IEnumerable<ProtoBufRule> ProtoBufRules = typeof(ProtoBufRule).Assembly
            .GetTypes()
            .Where(t => typeof(ProtoBufRule).IsAssignableFrom(t) && !t.IsAbstract)
            .Select(t => (ProtoBufRule)Activator.CreateInstance(t));

        public static IEnumerable<ProtoBufRule> GetProtoBufRules()
        {
            return ProtoBufRules;
        }
    }
}
