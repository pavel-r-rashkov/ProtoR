namespace ProtoR.Domain.ConfigurationSetAggregate
{
    using System;
    using ProtoR.Domain.GlobalConfigurationAggregate;
    using ProtoR.Domain.SeedWork;

    public interface IConfigurationSetRepository : IRepository<ConfigurationSet>
    {
        ConfigurationSet GetById(Guid id);

        void Update(ConfigurationSet configurationSet);

        void Add(ConfigurationSet configurationSet);
    }
}
