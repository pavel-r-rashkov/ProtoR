namespace ProtoR.Web.Infrastructure.Profiles
{
    using AutoMapper;
    using ProtoR.Application.Client;
    using ProtoR.Web.Resources.ClientResource;

    public class ClientProfile : Profile
    {
        public ClientProfile()
        {
            this.CreateMap<ClientDto, ClientReadModel>();

            this.CreateMap<ClientWriteModel, CreateClientCommand>();

            this.CreateMap<ClientWriteModel, UpdateClientCommand>();
        }
    }
}
