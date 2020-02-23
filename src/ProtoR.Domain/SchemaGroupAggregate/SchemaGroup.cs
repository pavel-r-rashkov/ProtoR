namespace ProtoR.Domain.SchemaGroupAggregate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ProtoR.Domain.GlobalConfigurationAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.SeedWork;
    using Version = ProtoR.Domain.SchemaGroupAggregate.Schemas.Version;

    public class SchemaGroup<TSchemaContents> : Entity, IAggregateRoot
    {
        private SortedSet<Schema<TSchemaContents>> schemas;
        private IEnumerable<Rule<TSchemaContents>> rules;

        public SchemaGroup(string name)
            : this(
                Guid.NewGuid(),
                name,
                new List<Schema<TSchemaContents>>(),
                new List<Rule<TSchemaContents>>())
        {
        }

        public SchemaGroup(
            Guid id,
            string name,
            IEnumerable<Schema<TSchemaContents>> schemas,
            IEnumerable<Rule<TSchemaContents>> rules)
            : base(id)
        {
            this.Name = name;
            this.schemas = new SortedSet<Schema<TSchemaContents>>(schemas, new SchemaVersionComparer<TSchemaContents>());
            this.rules = rules;
        }

        public string Name { get; }

        public IEnumerable<RuleViolation> AddSchema(
            string schemaContents,
            ConfigurationSet config,
            ISchemaFactory<TSchemaContents> schemaFactory)
        {
            if (config == null)
            {
                throw new ArgumentNullException($"{nameof(config)} cannot be null");
            }

            if (schemaFactory == null)
            {
                throw new ArgumentNullException($"{nameof(schemaFactory)} cannot be null");
            }

            if (config.SchemaGroupId != this.Id)
            {
                throw new ArgumentException($"Config with schema group id {config.SchemaGroupId} cannot be used for schema group with id {this.Id}");
            }

            Schema<TSchemaContents> lastSchema = this.schemas.LastOrDefault();
            Version newVersion = lastSchema?.Version.Next() ?? Version.Initial;
            Schema<TSchemaContents> newSchema = schemaFactory.CreateNew(newVersion, schemaContents);

            var ruleViolations = new List<RuleViolation>();
            IReadOnlyDictionary<RuleCode, RuleConfig> rulesConfig = config.GetRulesConfiguration();

            if (lastSchema != null)
            {
                var schemaEnumerator = this.schemas.Reverse().GetEnumerator();

                do
                {
                    Schema<TSchemaContents> oldSchema = schemaEnumerator.Current;

                    if (config.BackwardCompatible)
                    {
                        ruleViolations.AddRange(this.ValidateSchemas(newSchema, oldSchema, rulesConfig, true));
                    }

                    if (config.ForwardCompatible)
                    {
                        ruleViolations.AddRange(this.ValidateSchemas(newSchema, oldSchema, rulesConfig, false));
                    }
                }
                while (schemaEnumerator.MoveNext() && config.Transitive);
            }

            IEnumerable<RuleViolation> fatalViolations = ruleViolations.Where(ruleViolation => ruleViolation.Severity.IsFatal);

            if (!fatalViolations.Any())
            {
                this.schemas.Add(newSchema);
            }

            return ruleViolations;
        }

        private List<RuleViolation> ValidateSchemas(
            Schema<TSchemaContents> newSchema,
            Schema<TSchemaContents> oldSchema,
            IReadOnlyDictionary<RuleCode, RuleConfig> rulesConfig,
            bool backwardCompatible)
        {
            var ruleViolations = new List<RuleViolation>();
            var firstSchema = backwardCompatible ? newSchema : oldSchema;
            var secondSchema = backwardCompatible ? oldSchema : newSchema;

            foreach (var rule in this.rules.Where(rule => rulesConfig[rule.Code].Severity > Severity.Hidden))
            {
                ValidationResult validationResult = rule.Validate(firstSchema, secondSchema);

                if (!validationResult.Passed)
                {
                    ruleViolations.Add(new RuleViolation(
                        validationResult,
                        rulesConfig[rule.Code].Severity,
                        newSchema.Version,
                        oldSchema.Version,
                        backwardCompatible));
                }
            }

            return ruleViolations;
        }
    }
}
