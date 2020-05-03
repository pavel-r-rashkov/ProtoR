namespace ProtoR.Domain.SchemaGroupAggregate
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ProtoR.Domain.ConfigurationAggregate;
    using ProtoR.Domain.SchemaGroupAggregate.Rules;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.SeedWork;
    using Version = ProtoR.Domain.SchemaGroupAggregate.Schemas.Version;

    public abstract class SchemaGroup<TSchema, TSchemaContents> : Entity, IAggregateRoot
        where TSchema : Schema<TSchemaContents>
    {
        private readonly SortedSet<TSchema> schemas;
        private readonly IEnumerable<Rule<TSchema, TSchemaContents>> rules;
        private readonly ISchemaFactory<TSchema, TSchemaContents> schemaFactory;

        public SchemaGroup(
            string name,
            IEnumerable<Rule<TSchema, TSchemaContents>> rules,
            ISchemaFactory<TSchema, TSchemaContents> schemaFactory)
            : this(
                default,
                name,
                new List<TSchema>(),
                rules,
                schemaFactory)
        {
        }

        public SchemaGroup(
            long id,
            string name,
            IEnumerable<TSchema> schemas,
            IEnumerable<Rule<TSchema, TSchemaContents>> rules,
            ISchemaFactory<TSchema, TSchemaContents> schemaFactory)
            : base(id)
        {
            this.Name = name;
            this.schemas = new SortedSet<TSchema>(schemas, new SchemaVersionComparer<TSchemaContents>());
            this.rules = rules;
            this.schemaFactory = schemaFactory;
        }

        public string Name { get; }

        public IReadOnlyList<Schema<TSchemaContents>> Schemas
        {
            get { return this.schemas.ToList().AsReadOnly(); }
        }

        public IEnumerable<RuleViolation> AddSchema(
            string schemaContents,
            GroupConfiguration groupConfiguration,
            IReadOnlyDictionary<RuleCode, RuleConfiguration> rulesConfiguration)
        {
            var (ruleViolations, newSchema) = this.TestSchema(schemaContents, groupConfiguration, rulesConfiguration);
            IEnumerable<RuleViolation> fatalViolations = ruleViolations.Where(ruleViolation => ruleViolation.Severity.IsFatal);

            if (!fatalViolations.Any())
            {
                this.schemas.Add(newSchema);
            }

            return ruleViolations;
        }

        public (IEnumerable<RuleViolation>, TSchema) TestSchema(
            string schemaContents,
            GroupConfiguration groupConfiguration,
            IReadOnlyDictionary<RuleCode, RuleConfiguration> rulesConfiguration)
        {
            if (groupConfiguration == null)
            {
                throw new ArgumentNullException($"{nameof(groupConfiguration)} cannot be null");
            }

            if (rulesConfiguration == null)
            {
                throw new ArgumentNullException($"{nameof(rulesConfiguration)} cannot be null");
            }

            TSchema lastSchema = this.schemas.LastOrDefault();
            Version newVersion = lastSchema?.Version.Next() ?? Version.Initial;
            TSchema newSchema = this.schemaFactory.CreateNew(newVersion, schemaContents);

            var ruleViolations = new List<RuleViolation>();

            if (lastSchema != null)
            {
                var schemaEnumerator = this.schemas.Reverse().GetEnumerator();
                schemaEnumerator.MoveNext();

                do
                {
                    TSchema oldSchema = schemaEnumerator.Current;

                    if (groupConfiguration.BackwardCompatible)
                    {
                        ruleViolations.AddRange(this.ValidateSchemas(newSchema, oldSchema, rulesConfiguration, true));
                    }

                    if (groupConfiguration.ForwardCompatible)
                    {
                        ruleViolations.AddRange(this.ValidateSchemas(newSchema, oldSchema, rulesConfiguration, false));
                    }
                }
                while (schemaEnumerator.MoveNext() && groupConfiguration.Transitive);
            }

            return (ruleViolations, newSchema);
        }

        private List<RuleViolation> ValidateSchemas(
            TSchema newSchema,
            TSchema oldSchema,
            IReadOnlyDictionary<RuleCode, RuleConfiguration> rulesConfiguration,
            bool backwardCompatible)
        {
            var ruleViolations = new List<RuleViolation>();
            var firstSchema = backwardCompatible ? newSchema : oldSchema;
            var secondSchema = backwardCompatible ? oldSchema : newSchema;

            foreach (var rule in this.rules.Where(rule => rulesConfiguration[rule.Code].Severity > Severity.Hidden))
            {
                ValidationResult validationResult = rule.Validate(firstSchema, secondSchema);

                if (!validationResult.Passed)
                {
                    ruleViolations.Add(new RuleViolation(
                        validationResult,
                        rulesConfiguration[rule.Code].Severity,
                        newSchema.Version,
                        oldSchema.Version,
                        backwardCompatible));
                }
            }

            return ruleViolations;
        }
    }
}
