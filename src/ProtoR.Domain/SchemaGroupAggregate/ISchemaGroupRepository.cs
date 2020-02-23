namespace ProtoR.Domain.SchemaGroupAggregate
{
    using ProtoR.Domain.SeedWork;

    public interface ISchemaGroupRepository<TSchemaContents> : IRepository<SchemaGroup<TSchemaContents>>
    {
        SchemaGroup<TSchemaContents> GetByName(string name);

        void Update(SchemaGroup<TSchemaContents> schemaGroup);

        void Add(SchemaGroup<TSchemaContents> schemaGroup);
    }
}
