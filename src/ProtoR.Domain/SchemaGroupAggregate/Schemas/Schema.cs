namespace ProtoR.Domain.SchemaGroupAggregate.Schemas
{
    using System;
    using ProtoR.Domain.SeedWork;

    public abstract class Schema<TSchemaContents> : Entity
    {
        private readonly Lazy<TSchemaContents> parsedSchema;

        public Schema(Guid id, Version version, string contents)
            : base(id)
        {
            this.Version = version;
            this.Contents = contents;
            this.parsedSchema = new Lazy<TSchemaContents>(this.ParseContents);
        }

        public Version Version { get; }

        public string Contents { get; }

        public TSchemaContents Parsed => this.parsedSchema.Value;

        protected abstract TSchemaContents ParseContents();
    }
}
