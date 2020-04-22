namespace ProtoR.Domain.ConfigurationAggregate
{
    using System.Threading.Tasks;
    using ProtoR.Domain.SeedWork;

    public interface IConfigurationRepository : IRepository<Configuration>
    {
        Task<Configuration> GetById(long id);

        Task<Configuration> GetBySchemaGroupId(long? groupId);

        Task Update(Configuration configuration);

        Task<long> Add(Configuration configuration);
    }
}
