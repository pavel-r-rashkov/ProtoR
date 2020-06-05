namespace ProtoR.Application.Client
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IClientDataProvider
    {
        Task<ClientDto> GetById(long id);

        Task<IEnumerable<ClientDto>> GetClients();

        Task<IEnumerable<string>> GetOrigins();
    }
}
