namespace ProtoR.Domain.ClientAggregate
{
    using System.Threading.Tasks;

    public interface IClientRepository
    {
        Task<Client> GetById(long id);

        Task<Client> GetByClientId(string clientId);

        Task<long> Add(Client client);

        Task Update(Client client);

        Task Delete(long id);
    }
}
