namespace ProtoR.Application.Configuration
{
    using System.Threading.Tasks;

    public interface IConfigurationDataProvider
    {
        Task<ConfigurationDto> GetById(long id);

        Task<ConfigurationDto> GetGlobalConfig();

        Task<ConfigurationDto> GetConfigByGroupName(string groupName);
    }
}
