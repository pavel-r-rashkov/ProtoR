namespace ProtoR.Domain.ConfigurationSetAggregate
{
    using System;
    using System.Threading.Tasks;
    using ProtoR.Domain.GlobalConfigurationAggregate;
    using ProtoR.Domain.SeedWork;

    public interface IConfigurationSetRepository : IRepository<ConfigurationSet>
    {
        Task<ConfigurationSet> GetById(long id);

        Task<ConfigurationSet> GetBySchemaGroupId(long? groupId);

        Task Update(ConfigurationSet configurationSet);

        Task<long> Add(ConfigurationSet configurationSet);
    }
}
