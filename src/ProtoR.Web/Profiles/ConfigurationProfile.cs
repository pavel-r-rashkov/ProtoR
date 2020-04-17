namespace ProtoR.Web.Profiles
{
    using AutoMapper;
    using ProtoR.Application.Configuration;
    using ProtoR.Web.Resources.ConfigurationResource;

    public class ConfigurationProfile : Profile
    {
        public ConfigurationProfile()
        {
            this.CreateMap<ConfigurationDto, ConfigurationReadModel>();

            this.CreateMap<ConfigurationWriteModel, UpdateConfigurationCommand>();
        }
    }
}
