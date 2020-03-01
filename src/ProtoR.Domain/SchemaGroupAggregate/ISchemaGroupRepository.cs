namespace ProtoR.Domain.SchemaGroupAggregate
{
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.SeedWork;

    public interface ISchemaGroupRepository<TSchema, TSchemaContents> : IRepository<SchemaGroup<TSchema, TSchemaContents>>
        where TSchema : Schema<TSchemaContents>
    {
        SchemaGroup<TSchema, TSchemaContents> GetByName(string name);

        void Update(SchemaGroup<TSchema, TSchemaContents> schemaGroup);

        void Add(SchemaGroup<TSchema, TSchemaContents> schemaGroup);
    }
}
