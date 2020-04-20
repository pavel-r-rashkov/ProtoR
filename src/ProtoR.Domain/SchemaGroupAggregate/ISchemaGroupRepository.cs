namespace ProtoR.Domain.SchemaGroupAggregate
{
    using System.Threading.Tasks;
    using ProtoR.Domain.SchemaGroupAggregate.Schemas;
    using ProtoR.Domain.SeedWork;

    public interface ISchemaGroupRepository<TSchema, TSchemaContents> : IRepository<SchemaGroup<TSchema, TSchemaContents>>
        where TSchema : Schema<TSchemaContents>
    {
        Task<SchemaGroup<TSchema, TSchemaContents>> GetByName(string name);

        Task Update(SchemaGroup<TSchema, TSchemaContents> schemaGroup);

        Task<long> Add(SchemaGroup<TSchema, TSchemaContents> schemaGroup);
    }
}
