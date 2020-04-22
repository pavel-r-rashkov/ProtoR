namespace ProtoR.Domain.SchemaGroupAggregate
{
    using System.Threading.Tasks;
    using ProtoR.Domain.SeedWork;

    public interface IProtoBufSchemaGroupRepository : IRepository<ProtoBufSchemaGroup>
    {
        Task<ProtoBufSchemaGroup> GetByName(string name);

        Task Update(ProtoBufSchemaGroup schemaGroup);

        Task<long> Add(ProtoBufSchemaGroup schemaGroup);
    }
}
